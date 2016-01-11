using System;
using System.IO;
using System.Reflection;
using Tests.Framework.Integration;
using Tests.Profiling;

namespace Profiling.Performance
{
    internal class PerformanceProfileFactory : ProfileFactory<PerformanceAttribute>
    {
        public PerformanceProfileFactory(
            string sdkPath, 
            string outputPath, 
            ClusterBase cluster, 
            Assembly assembly, 
            TextWriter output = null) : base(sdkPath, outputPath, cluster, assembly, output)
        {
        }

        protected override IDisposable BeginProfiling(string resultsDirectory)
        {
            return new PerformanceProfile(SdkPath, resultsDirectory);
        }
    }
}