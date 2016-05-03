using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Elasticsearch.Net;
using Nest;
using Tests.Framework;

namespace Tests.Document.Multiple.Bulk
{
	public class BulkBenchmarkTests
	{
		private static readonly IElasticClient Client = TestClient.GetInMemoryClient();
		private byte[] _tinyResponse;
		private byte[] _mediumResponse;
		private byte[] _largeResponse;
		private byte[] _hugeResponse;

		[Setup]
		public void Setup()
		{
			var serializer = Client.Serializer;
			_tinyResponse = serializer.SerializeToBytes(ReturnBulkResponse(1));
			_mediumResponse = serializer.SerializeToBytes(ReturnBulkResponse(100));
			_largeResponse = serializer.SerializeToBytes(ReturnBulkResponse(1000));
			_hugeResponse = serializer.SerializeToBytes(ReturnBulkResponse(100000));
		}

		[Benchmark(Description = "deserialize 1 item in bulk response")]
		public BulkResponse TinyResponse()
		{
			using (var ms = new MemoryStream(_tinyResponse))
				return Client.Serializer.Deserialize<BulkResponse>(ms);
		}

		[Benchmark(Description = "deserialize 100 items in bulk response")]
		public BulkResponse MediumResponse()
		{
			using (var ms = new MemoryStream(_mediumResponse))
				return Client.Serializer.Deserialize<BulkResponse>(ms);
		}

		[Benchmark(Description = "deserialize 1,000 items in bulk response")]
		public BulkResponse LargeResponse()
		{
			using (var ms = new MemoryStream(_largeResponse))
				return Client.Serializer.Deserialize<BulkResponse>(ms);
		}

		[Benchmark(Description = "deserialize 100,000 items in bulk response")]
		public BulkResponse HugeResponse()
		{
			using (var ms = new MemoryStream(_hugeResponse))
				return Client.Serializer.Deserialize<BulkResponse>(ms);
		}

		private static object BulkItemResponse() => new
		{
			index = new
			{
				_index = "nest-52cfd7aa",
				_type = "project",
				_id = "Kuhn LLC",
				_version = 1,
				_shards = new
				{
					total = 2,
					successful = 1,
					failed = 0
				},
				created = true,
				status = 201
			}
		};

		private static object ReturnBulkResponse(int numberOfItems)
		{
			return new
			{
				took = 276,
				errors = false,
				items = Enumerable.Range(0, numberOfItems)
					.Select(i => BulkItemResponse())
					.ToArray()
			};
		}
	}
}
