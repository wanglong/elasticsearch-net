using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;

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
