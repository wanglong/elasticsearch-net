using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Tests.Framework.Integration;
using Tests.Framework.Profiling;

namespace Profiling
{
	internal abstract class ProfileFactory<TAttribute> : IProfileFactory where TAttribute : ProfilingAttribute
	{
		private IEnumerable<ProfiledClass> _profiledClasses;

		protected ProfileFactory(string sdkPath,
			string outputPath,
			ClusterBase cluster,
			Assembly assembly,
			IColoredWriter output)
		{
			SdkPath = sdkPath;
			OutputPath = Path.Combine(
				outputPath,
				this.GetType().Name.Replace("ProfileFactory", string.Empty).ToLowerInvariant());
			Cluster = cluster;
			Assembly = assembly;
			Output = output;
		}

		protected Assembly Assembly { get; }

		protected ClusterBase Cluster { get; }

		protected IColoredWriter Output { get; }

		protected string OutputPath { get; }

		protected IEnumerable<ProfiledClass> ProfiledClasses
		{
			get
			{
				if (_profiledClasses == null)
				{
					_profiledClasses = Assembly
						.GetTypes()
						.Where(t => t.IsPublic && t.IsClass)
						.Where(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public)
							.Any(m => m.GetCustomAttribute<TAttribute>() != null))
						.Select(t =>
						{
							var methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public)
								.Where(m => m.GetCustomAttribute<TAttribute>() != null)
								.Select(m => new ProfiledMethod(m, m.GetCustomAttribute<TAttribute>()));

							return new ProfiledClass(t, methods);
						});
				}

				return _profiledClasses;
			}
		}

		protected string SdkPath { get; }

		public void Run()
		{
			foreach (var profiledClass in ProfiledClasses)
			{
				var instance = profiledClass.CreateInstance(Cluster);

				foreach (var profiledMethod in profiledClass.Methods.Where(m => !m.IsAsync))
				{
					var resultsDirectory = Path.Combine(OutputPath, profiledClass.Type.Name, profiledMethod.MethodInfo.Name);
					var action = profiledMethod.Compile(instance);

					Output.WriteLine(ConsoleColor.Green, $"Profiling {profiledClass.Type.Name}.{profiledMethod.MethodInfo.Name}");

					using (BeginProfiling(resultsDirectory))
					{
						for (int i = 0; i < profiledMethod.Attribute.Iterations; i++)
						{
							action();
						}
					}
				}
			}
		}

		public async Task RunAsync()
		{
			foreach (var profiledClass in ProfiledClasses)
			{
				var instance = profiledClass.CreateInstance(Cluster);

				foreach (var profiledMethod in profiledClass.Methods.Where(m => m.IsAsync))
				{
					var resultsDirectory = Path.Combine(OutputPath, profiledClass.Type.Name, profiledMethod.MethodInfo.Name);
					var thunk = profiledMethod.CompileAsync(instance);

					Output.WriteLine(ConsoleColor.Green, $"profiling {profiledClass.Type.Name}.{profiledMethod.MethodInfo.Name}");

					using (BeginProfiling(resultsDirectory))
					{
						for (int i = 0; i < profiledMethod.Attribute.Iterations; i++)
						{
							await thunk();
						}
					}
				}
			}
		}

		protected abstract IDisposable BeginProfiling(string resultsDirectory);
	}
}
