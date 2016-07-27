using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Tests.Framework;

namespace Tests
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var benchmarkSwitcher = new BenchmarkSwitcher(GetBenchmarkTypes());
			benchmarkSwitcher.Run(args);
		}

		private static Type[] GetBenchmarkTypes()
		{
			IEnumerable<Type> types;

			try
			{
				types = typeof(Program).Assembly().GetTypes();
			}
			catch (ReflectionTypeLoadException e)
			{
				types = e.Types.Where(t => t != null);
			}

			return types
				.Where(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public)
							 .Any(m => m.GetCustomAttributes(typeof(BenchmarkAttribute), false).Any()))
				.OrderBy(t => t.Namespace)
				.ThenBy(t => t.Name)
				.ToArray();
		}
	}

	public class FastRunConfig : ManualConfig
	{
		public FastRunConfig()
		{
			Add(Job.Dry.With(Runtime.Core).With(Jit.Host));
			Add(Job.Dry.With(Runtime.Clr).With(Jit.Host));
			Add(AsciiDocExporter.Default);
		}
	}
}
