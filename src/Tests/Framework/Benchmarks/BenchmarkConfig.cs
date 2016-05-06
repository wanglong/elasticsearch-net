using BenchmarkDotNet.Configs;

namespace Tests.Framework.Benchmarks
{
	public class BenchmarkConfig : ManualConfig
	{
		public BenchmarkConfig()
		{
			Add(AsciiDocExporter.Default);
		}
	}
}
