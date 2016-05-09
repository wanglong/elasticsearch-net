using Xunit;

namespace Tests.Framework.Integration.Clusters
{
	[CollectionDefinition(IntegrationContext.Benchmark)]
	public class BenchmarkCluster : ClusterBase, ICollectionFixture<BenchmarkCluster>, IClassFixture<EndpointUsage>
	{
	}
}
