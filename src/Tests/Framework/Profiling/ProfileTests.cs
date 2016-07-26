using System;
using Elasticsearch.Net;
using Nest;

namespace Tests.Framework.Profiling
{
	public abstract class ProfileTests
	{
		public static readonly ElasticClient InMemoryClient;

		static ProfileTests()
		{
			var connectionPool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
			var connectionSettings = new ConnectionSettings(connectionPool, new InMemoryConnection());
			InMemoryClient = new ElasticClient(connectionSettings);
		}
	}
}
