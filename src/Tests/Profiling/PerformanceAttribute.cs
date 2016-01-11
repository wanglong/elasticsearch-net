namespace Tests.Profiling
{
    /// <summary>
    /// identifies a method that should be performance profiled
    /// </summary>
    public class PerformanceAttribute : ProfilingAttribute
    {
        public PerformanceAttribute(int iterations = 1) : base(iterations)
        {
        }
    }
}