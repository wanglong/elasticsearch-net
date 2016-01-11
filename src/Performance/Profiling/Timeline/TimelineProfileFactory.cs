using System;
using System.IO;
using System.Reflection;
using Tests.Framework.Integration;
using Tests.Profiling;

namespace Profiling.Timeline
{
    internal class TimelineProfileFactory : ProfileFactory<TimelineAttribute>
    {
        public TimelineProfileFactory(
            string sdkPath,
            string outputPath,
            ClusterBase cluster,
            Assembly assembly,
            TextWriter output = null) : base(sdkPath, outputPath, cluster, assembly, output)
        { }

        protected override IDisposable BeginProfiling(string resultsDirectory)
        {
            return new TimelineProfile(SdkPath, resultsDirectory);
        }
    }
}