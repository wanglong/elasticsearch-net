using Tests.Framework.Configuration;
using Tests.Framework.Versions;

namespace Profiling
{
	public class ProfilingTestConfiguration : YamlConfiguration
	{
		public override bool RunIntegrationTests => true;

		public override ElasticsearchVersion ElasticsearchVersion => new ElasticsearchVersion("5.0.0-alpha3");

		public override bool TestAgainstAlreadyRunningElasticsearch { get; protected set; } = true;

		public ProfilingTestConfiguration()
			: base(@"..\..\..\..\Tests\tests.yaml")
		{
		}
	}
}

