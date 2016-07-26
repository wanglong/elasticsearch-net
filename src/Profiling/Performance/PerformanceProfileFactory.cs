using System;
using System.IO;
using System.Reflection;
using Tests.Framework.Integration;
using Tests.Framework.Profiling;

namespace Profiling.Performance
{
	internal class PerformanceProfileFactory : ProfileFactory<PerformanceAttribute>
	{
		public PerformanceProfileFactory(
			string sdkPath,
			string outputPath,
			ClusterBase cluster,
			Assembly assembly,
			IColoredWriter output) : base(sdkPath, outputPath, cluster, assembly, output)
		{
		}

		protected override IDisposable BeginProfiling(string resultsDirectory)
		{
			return new PerformanceProfile(SdkPath, resultsDirectory);
		}
	}
}
