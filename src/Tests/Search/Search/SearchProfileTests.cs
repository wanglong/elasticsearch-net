using Nest;
using Tests.Framework.Integration;
using Tests.Framework.MockData;
using Tests.Framework.Profiling;

namespace Tests.Search.Search
{
    public class SearchProfileTests : ProfileTests
    {
        private readonly IElasticClient _client;

        public SearchProfileTests(IIntegrationCluster cluster)
        {
            _client = cluster.Client(s => s);
        }

        [Timeline(1000)]
        public void Deserialization()
        {
            _client.Search<Developer>(s => s.Query(q => q.MatchAll()));
        }

        [Timeline(1000)]
        public void Serialization()
        {
            InMemoryClient.Search<Developer>();
        }
    }
}
