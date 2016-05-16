using System;

namespace Tests.Framework.Profiling
{
    /// <summary>
    /// identifies a method that should be timeline profiled
    /// </summary>
    public class TimelineAttribute : ProfilingAttribute
    {
        public TimelineAttribute(int iterations = 1) : base(iterations)
        {
        }
    }
}
