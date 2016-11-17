using System;
using System.Collections.Generic;
using System.Linq;
using Elasticsearch.Net;
using FluentAssertions;
using Nest;
using Newtonsoft.Json.Linq;
using Tests.Framework;
using Tests.Framework.Integration;
using Xunit;

namespace Tests.XPack.Watcher.ExecuteWatch
{
	public class ExecuteWatchApiTests : ApiIntegrationTestBase<XPackCluster, IExecuteWatchResponse, IExecuteWatchRequest, ExecuteWatchDescriptor, ExecuteWatchRequest>
	{
		private readonly DateTime _triggeredDateTime = new DateTime(2016, 11, 17, 13, 00, 00);

		public ExecuteWatchApiTests(XPackCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override void IntegrationSetup(IElasticClient client, CallUniqueValues values)
		{
			foreach (var callUniqueValue in values)
			{
				var putWatchResponse = client.PutWatch(callUniqueValue.Value, p => p
					.Input(i => i
						.Search(s => s
							.Request(r => r
								.Indices("logstash")
								.Body<object>(b => b
									.Query(q => q
										.Match(m => m
												.Field("response")
												.Query("404")
										) && +q
										.DateRange(ffrr => ffrr
											.Field("@timestamp")
											.GreaterThanOrEquals("{{ctx.trigger.scheduled_time}}||-5m")
											.LessThanOrEquals("{{ctx.trigger.triggered_time}}")
										)
									)
								)
							)
						)
					)
					.Condition(c => c
						.Script(ss => ss
							.Inline("ctx.payload.hits.total > 1")
						)
					)
					.Trigger(t => t
						.Schedule(s => s
							.Cron("0 0 0 1 * ? 2099")
						)
					)
					.Metadata(meta => meta.Add("foo", "bar"))
					.Actions(a => a
						.Email("email_admin", e => e
							.To("someone@domain.host.com")
							.Subject("404 recently encountered")
						)
						.Index("index_action", i => i
							.Index("test")
							.DocType("doctype2")
						)
						.Logging("logging_action", l => l
							.Text("404 recently encountered")
						)
						.Webhook("webhook_action", w => w
							.Host("foo.com")
							.Port(80)
							.Path("/bar")
							.Method(HttpInputMethod.Post)
							.Body("{}")
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

		protected override bool SupportsDeserialization => false;

		protected override object ExpectJson => new
			{
				action_modes = new
				{
					email_admin = "force_simulate",
					webhook_action = "force_simulate"
				},
				alternative_input = new
				{
					foo = "bar"
				},
				ignore_condition = true,
				record_execution = true,
				trigger_data = new
				{
					scheduled_time = "2016-11-17T13:00:00",
					triggered_time = "2016-11-17T13:00:00"
				}
			};

		protected override Func<ExecuteWatchDescriptor, IExecuteWatchRequest> Fluent => f => f
			.Id(CallIsolatedValue)
			.TriggerData(te => te
				.ScheduledTime(_triggeredDateTime)
				.TriggeredTime(_triggeredDateTime)
			)
			.AlternativeInput(i => i.Add("foo", "bar"))
			.IgnoreCondition()
			.ActionModes(a => a
				.Add("email_admin", ActionExecutionMode.ForceSimulate)
				.Add("webhook_action", ActionExecutionMode.ForceSimulate)
			)
			.RecordExecution();


		protected override ExecuteWatchRequest Initializer =>
			new ExecuteWatchRequest(CallIsolatedValue)
			{
				TriggerData = new ScheduleTriggerEvent
				{
					ScheduledTime = _triggeredDateTime,
					TriggeredTime = _triggeredDateTime
				},
				AlternativeInput = new Dictionary<string, object>
				{
					{ "foo", "bar" }
				},
				IgnoreCondition = true,
				ActionModes = new Dictionary<string, ActionExecutionMode>
				{
					{ "email_admin", ActionExecutionMode.ForceSimulate },
					{ "webhook_action", ActionExecutionMode.ForceSimulate },
				},
				RecordExecution = true
			};

		protected override void ExpectResponse(IExecuteWatchResponse response)
		{
			response.IsValid.Should().BeTrue();
			response.WatchRecord.Should().NotBeNull();
			response.WatchRecord.WatchId.Should().Be(new Id(CallIsolatedValue));
			response.WatchRecord.State.Should().NotBeNull().And.Be(ActionExecutionState.Executed);
			response.WatchRecord.TriggerEvent.Should().NotBeNull();
			response.WatchRecord.TriggerEvent.Type.Should().Be("manual");
			response.WatchRecord.TriggerEvent.TriggeredTime.Should().Be(_triggeredDateTime);
			response.WatchRecord.TriggerEvent.Manual.Should().NotBeNull();
			response.WatchRecord.TriggerEvent.Manual.Schedule.Should().NotBeNull();
			response.WatchRecord.TriggerEvent.Manual.Schedule.ScheduledTime.Match(
				f => f.Should().Be(_triggeredDateTime),
				s => Assert.True(false, "expected a datetime")
			);

			response.WatchRecord.Result.Should().NotBeNull();
			response.WatchRecord.Result.Condition.Should().NotBeNull();
			response.WatchRecord.Result.Condition.Type.Should().Be(ConditionType.Always);
			response.WatchRecord.Result.Condition.Status.Should().Be(Status.Success);
			response.WatchRecord.Result.Condition.Met.Should().BeTrue();

			response.WatchRecord.Result.Actions.Should().NotBeNullOrEmpty();
			response.WatchRecord.Result.Actions.Count.Should().Be(4);

			var inputContainer = response.WatchRecord.Input as IInputContainer;
			inputContainer.Should().NotBeNull();
			inputContainer.Search.Should().NotBeNull();

			response.WatchRecord.Metadata.Should().NotBeNull();
			response.WatchRecord.Metadata.Should().Contain("foo", "bar");

			var emailAction = response.WatchRecord.Result.Actions.FirstOrDefault(a => a.Id == "email_admin");
			emailAction.Should().NotBeNull();
			emailAction.Type.Should().Be(ActionType.Email);
			emailAction.Status.Should().Be(Status.Simulated);
			emailAction.Email.Should().NotBeNull();
			emailAction.Email.Message.SentDate.Should().HaveValue();

			var indexAction = response.WatchRecord.Result.Actions.FirstOrDefault(a => a.Id == "index_action");
			indexAction.Should().NotBeNull();
			indexAction.Type.Should().Be(ActionType.Index);
			indexAction.Status.Should().Be(Status.Success);
			indexAction.Index.Response.Should().NotBeNull();
			indexAction.Index.Response.Index.Should().Be("test");
			indexAction.Index.Response.Type.Should().Be("doctype2");
			indexAction.Index.Response.Created.Should().BeTrue();
			indexAction.Index.Response.Version.Should().Be(1);

			var loggingAction = response.WatchRecord.Result.Actions.FirstOrDefault(a => a.Id == "logging_action");
			loggingAction.Should().NotBeNull();
			loggingAction.Type.Should().Be(ActionType.Logging);
			loggingAction.Status.Should().Be(Status.Success);
			loggingAction.Logging.LoggedText.Should().Be("404 recently encountered");

			var webhookAction = response.WatchRecord.Result.Actions.FirstOrDefault(a => a.Id == "webhook_action");
			webhookAction.Should().NotBeNull();
			webhookAction.Type.Should().Be(ActionType.Webhook);
			webhookAction.Status.Should().Be(Status.Simulated);
			webhookAction.Webhook.Should().NotBeNull();

			response.WatchRecord.Result.ExecutionTime.Should().HaveValue();

		}
	}

	public class ExecuteInlineWatchApiTests : ApiIntegrationTestBase<XPackCluster, IExecuteWatchResponse, IExecuteWatchRequest, ExecuteWatchDescriptor, ExecuteWatchRequest>
	{
		private readonly DateTime _triggeredDateTime = new DateTime(2016, 11, 17, 13, 00, 00);

		public ExecuteInlineWatchApiTests(XPackCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override void IntegrationSetup(IElasticClient client, CallUniqueValues values)
		{
			foreach (var callUniqueValue in values)
			{
				var putWatchResponse = client.PutWatch(callUniqueValue.Value, p => p
					.Input(i => i
						.Search(s => s
							.Request(r => r
								.Indices("logstash")
								.Body<object>(b => b
									.Query(q => q
										.Match(m => m
												.Field("response")
												.Query("404")
										) && +q
										.DateRange(ffrr => ffrr
											.Field("@timestamp")
											.GreaterThanOrEquals("{{ctx.trigger.scheduled_time}}||-5m")
											.LessThanOrEquals("{{ctx.trigger.triggered_time}}")
										)
									)
								)
							)
						)
					)
					.Condition(c => c
						.Script(ss => ss
							.Inline("ctx.payload.hits.total > 1")
						)
					)
					.Trigger(t => t
						.Schedule(s => s
							.Cron("0 0 0 1 * ? 2099")
						)
					)
					.Metadata(meta => meta.Add("foo", "bar"))
					.Actions(a => a
						.Email("email_admin", e => e
							.To("someone@domain.host.com")
							.Subject("404 recently encountered")
						)
						.Index("index_action", i => i
							.Index("test")
							.DocType("doctype2")
						)
						.Logging("logging_action", l => l
							.Text("404 recently encountered")
						)
						.Webhook("webhook_action", w => w
							.Host("foo.com")
							.Port(80)
							.Path("/bar")
							.Method(HttpInputMethod.Post)
							.Body("{}")
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

		protected override string UrlPath => $"/_xpack/watcher/watch/_execute";

		protected override bool SupportsDeserialization => false;

		protected override object ExpectJson => new
		{
			alternative_input = new
			{
				foo = "bar"
			},
			ignore_condition = true,
			trigger_data = new
			{
				scheduled_time = "2016-11-17T13:00:00",
				triggered_time = "2016-11-17T13:00:00"
			},
			watch = new
			{
				actions = new
				{
					email_admin = new
					{
						email = new
						{
							from = "nest-client@domain.example",
							subject = "404 recently encountered",
							to = new[] { "someone@domain.host.example" }
						}
					},
					index_action = new
					{
						index = new
						{
							doc_type = "doctype2",
							index = "test"
						}
					},
					logging_action = new
					{
						logging = new
						{
							text = "404 recently encountered"
						}
					},
					webhook_action = new
					{
						webhook = new
						{
							body = "{}",
							host = "foo.com",
							method = "post",
							path = "/bar",
							port = 80
						}
					}
				},
				condition = new
				{
					script = new
					{
						inline = "ctx.payload.hits.total > 1"
					}
				},
				input = new
				{
					search = new
					{
						request = new
						{
							body = new
							{
								query = new
								{
									@bool = new
									{
										filter = new[]
										{
											new
											{
												range = new JObject
												{
													{
														"@timestamp", new JObject
														{
															{ "gte", "{{ctx.trigger.scheduled_time}}||-5m" },
															{ "lte", "{{ctx.trigger.triggered_time}}" }
														}
													}
												}
											}
										},
										must = new[]
										{
											new
											{
												match = new
												{
													response = new
													{
														query = "404"
													}
												}
											}
										}
									}
								}
							},
							indices = new[] { "logstash" }
						}
					}
				},
				metadata = new
				{
					foo = "bar"
				},
				trigger = new
				{
					schedule = new
					{
						cron = "0 0 0 1 * ? 2099"
					}
				}
			}
		};

		protected override Func<ExecuteWatchDescriptor, IExecuteWatchRequest> Fluent => f => f
			.TriggerData(te => te
				.ScheduledTime(_triggeredDateTime)
				.TriggeredTime(_triggeredDateTime)
			)
			.AlternativeInput(i => i.Add("foo", "bar"))
			.IgnoreCondition()
			.Watch(wa => wa
				.Input(i => i
					.Search(s => s
						.Request(r => r
							.Indices("logstash")
							.Body<object>(b => b
								.Query(q => q
									.Match(m => m
										.Field("response")
										.Query("404")
									) && +q
									.DateRange(ffrr => ffrr
										.Field("@timestamp")
										.GreaterThanOrEquals("{{ctx.trigger.scheduled_time}}||-5m")
										.LessThanOrEquals("{{ctx.trigger.triggered_time}}")
									)
								)
							)
						)
					)
				)
				.Condition(c => c
					.Script(ss => ss
						.Inline("ctx.payload.hits.total > 1")
					)
				)
				.Trigger(t => t
					.Schedule(s => s
						.Cron("0 0 0 1 * ? 2099")
					)
				)
				.Metadata(meta => meta.Add("foo", "bar"))
				.Actions(a => a
					.Email("email_admin", e => e
						.From("nest-client@domain.example")
						.To("someone@domain.host.example")
						.Subject("404 recently encountered")
					)
					.Index("index_action", i => i
						.Index("test")
						.DocType("doctype2")
					)
					.Logging("logging_action", l => l
						.Text("404 recently encountered")
					)
					.Webhook("webhook_action", w => w
						.Host("foo.com")
						.Port(80)
						.Path("/bar")
						.Method(HttpInputMethod.Post)
						.Body("{}")
					)
				)
			);



		protected override ExecuteWatchRequest Initializer =>
			new ExecuteWatchRequest
			{
				TriggerData = new ScheduleTriggerEvent
				{
					ScheduledTime = _triggeredDateTime,
					TriggeredTime = _triggeredDateTime
				},
				AlternativeInput = new Dictionary<string, object>
				{
					{ "foo", "bar" }
				},
				IgnoreCondition = true,
				Watch = new PutWatchRequest
				{
					Trigger = new ScheduleContainer
					{
						Cron = "0 0 0 1 * ? 2099"
					},
					Metadata = new Dictionary<string, object>
					{
						{ "foo", "bar" }
					},
					Input = new SearchInput
					{
						Request = new SearchInputRequest
						{
							Indices = new IndexName[] { "logstash" },
							Body = new SearchRequest
							{
								Query = new MatchQuery
								{
									Field = "response",
									Query = "404"
								} && +new DateRangeQuery
								{
									Field = "@timestamp",
									GreaterThanOrEqualTo = "{{ctx.trigger.scheduled_time}}||-5m",
									LessThanOrEqualTo = "{{ctx.trigger.triggered_time}}"
								}
							}
						}
					},
					Condition = new ScriptCondition
					{
						Inline = "ctx.payload.hits.total > 1"
					},
					Actions = new EmailAction("email_admin")
					{
						From = "nest-client@domain.example",
						To = new [] {"someone@domain.host.example"},
						Subject = "404 recently encountered"
					} && new IndexAction("index_action")
					{
						Index = "test",
						DocType = "doctype2"
					} && new LoggingAction("logging_action")
					{
						Text = "404 recently encountered"
					}
					&& new WebhookAction("webhook_action")
					{
						Host = "foo.com",
						Port = 80,
						Path = "/bar",
						Method = HttpInputMethod.Post,
						Body = "{}"
					}
				}
			};

		protected override void ExpectResponse(IExecuteWatchResponse response)
		{
			response.IsValid.Should().BeTrue();
			response.WatchRecord.TriggerEvent.Should().NotBeNull();
			response.WatchRecord.TriggerEvent.TriggeredTime.Should().Be(_triggeredDateTime);
			response.WatchRecord.TriggerEvent.Manual.Should().NotBeNull();
			response.WatchRecord.TriggerEvent.Manual.Schedule.Should().NotBeNull();
			response.WatchRecord.TriggerEvent.Manual.Schedule.ScheduledTime.Match(
				f => f.Should().Be(_triggeredDateTime),
				s => Assert.True(false, "expected a datetime")
			);

			response.WatchRecord.Result.Input.Type.Should().Be(InputType.Simple);
			response.WatchRecord.Result.Input.Payload.Should().NotBeEmpty();
			response.WatchRecord.Result.Input.Payload["foo"].As<string>().Should().Be("bar");
			response.WatchRecord.Result.Condition.Met.Should().BeTrue();
			response.WatchRecord.Result.Actions.Should().NotBeEmpty();

			var emailAction = response.WatchRecord.Result.Actions.FirstOrDefault(a => a.Id == "email_admin");
			emailAction.Should().NotBeNull();
		}
	}
}
