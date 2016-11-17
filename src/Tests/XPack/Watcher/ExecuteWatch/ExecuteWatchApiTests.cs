using System;
using Elasticsearch.Net;
using FluentAssertions;
using Nest;
using Tests.Framework;
using Tests.Framework.Integration;

namespace Tests.XPack.Watcher.ExecuteWatch
{
	public class ExecuteWatchApiTests : ApiIntegrationTestBase<XPackCluster, IExecuteWatchResponse, IExecuteWatchRequest, ExecuteWatchDescriptor, ExecuteWatchRequest>
	{
		public ExecuteWatchApiTests(XPackCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override void IntegrationSetup(IElasticClient client, CallUniqueValues values)
		{
			foreach (var callUniqueValue in values)
			{
				var putWatchResponse = client.PutWatch(callUniqueValue.Value, p => p
					.Input(i => i
						.Simple(s => s
							.Add("key", "value")
						)
					)
					.Trigger(t => t
						.Schedule(s => s
							.Cron("0 5 9 * * ?")
						)
					)
					.Actions(a => a
						.Email("reminder_email", e => e
							.To("me@example.com")
							.Subject("Something's strange in the neighbourhood")
							.Body(b => b
								.Text("Dear {{ctx.payload.name}}, by the time you read these lines, I'll be gone")
							)
						)
					)
				);

				if (!putWatchResponse.IsValid)
					throw new Exception("Problem setting up integration test");
			}
		}

		protected override LazyResponses ClientUsage() => Calls(
			fluent: (client, f) => client.ExecuteWatch(f),
			fluentAsync: (client, f) => client.ExecuteWatchAsync(f),
			request: (client, r) => client.ExecuteWatch(r),
			requestAsync: (client, r) => client.ExecuteWatchAsync(r)
		);

		protected override bool ExpectIsValid => true;
		protected override int ExpectStatusCode => 200;
		protected override HttpMethod HttpMethod => HttpMethod.PUT;

		protected override string UrlPath => $"/_xpack/watcher/watch/{CallIsolatedValue}/_execute";

		protected override bool SupportsDeserialization => true;

		protected override object ExpectJson => null;

		protected override Func<ExecuteWatchDescriptor, IExecuteWatchRequest> Fluent => f => f
			.Id(CallIsolatedValue);

		protected override ExecuteWatchRequest Initializer =>
			new ExecuteWatchRequest(CallIsolatedValue);

		protected override void ExpectResponse(IExecuteWatchResponse response)
		{
			// TODO: Implement
			response.Id.Should().Be(new Id(CallIsolatedValue));
		}
	}
}
