using System;
using System.Threading.Tasks;
using Nest;
using Tests.Framework.Integration;
using Tests.Framework.MockData;
using Tests.Profiling;

namespace Tests.Document.Multiple.Bulk
{
    public class BulkProfileTests
    {
        private readonly IElasticClient _client;
        private static readonly string IndexName = "bulk-profile";

        public BulkProfileTests(IIntegrationCluster cluster)
        {
            _client = cluster.Client(s => s);

            if (_client.IndexExists(IndexName).Exists)
                _client.DeleteIndex(IndexName);

            var createIndexResponse = _client.CreateIndex(IndexName);

            if (!createIndexResponse.IsValid)
                Console.WriteLine($"invalid response creating index: {createIndexResponse.ServerError?.Error?.Reason}");
        }

        [Performance(10)]
        public void Sync()
        {
            var bulkResponse = _client.Bulk(b => b
                .IndexMany(Developer.Generator.Generate(1000), (bd, d) => bd
                    .Index(IndexName)
                    .Document(d)
                ));

            if (!bulkResponse.IsValid)
                if (bulkResponse.Errors)
                    foreach (var error in bulkResponse.ItemsWithErrors)
                        Console.WriteLine($"error with id {error.Id}. message: {error.Error.Reason}");
        }

        [Performance(10)]
        public async Task Async()
        {
            var bulkResponse = await _client.BulkAsync(b => b
                .IndexMany(Developer.Generator.Generate(1000), (bd, d) => bd
                    .Index(IndexName)
                    .Document(d)
                )).ConfigureAwait(false);

            if (!bulkResponse.IsValid)
                if (bulkResponse.Errors)
                    foreach (var error in bulkResponse.ItemsWithErrors)
                        Console.WriteLine($"error with id {error.Id}. message: {error.Error.Reason}");
        }
    }
}
