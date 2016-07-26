using Xunit;

namespace Tests.Framework.Integration.Clusters
{
	[CollectionDefinition(TypeOfCluster.Benchmark)]
	public class BenchmarkCluster : ClusterBase, ICollectionFixture<BenchmarkCluster>, IClassFixture<EndpointUsage>
	{
	}
}
