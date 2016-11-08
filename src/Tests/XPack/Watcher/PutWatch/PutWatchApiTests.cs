using System;
using System.Collections.Generic;
using System.Linq;
using Elasticsearch.Net;
using FluentAssertions;
using Nest;
using Newtonsoft.Json.Linq;
using Tests.Framework;
using Tests.Framework.Integration;
using Tests.Framework.MockData;

namespace Tests.XPack.Watcher.PutWatch
{
	public class PutWatchApiTests : ApiIntegrationTestBase<XPackCluster, IPutWatchResponse, IPutWatchRequest, PutWatchDescriptor, PutWatchRequest>
	{
		public PutWatchApiTests(XPackCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override LazyResponses ClientUsage() => Calls(
			fluent: (client, f) => client.PutWatch(CallIsolatedValue, f),
			fluentAsync: (client, f) => client.PutWatchAsync(CallIsolatedValue, f),
			request: (client, r) => client.PutWatch(r),
			requestAsync: (client, r) => client.PutWatchAsync(r)
		);

		protected override bool ExpectIsValid => true;
		protected override int ExpectStatusCode => 201;
		protected override HttpMethod HttpMethod => HttpMethod.PUT;

		protected override string UrlPath => $"/_xpack/watcher/watch/{CallIsolatedValue}";

		// TODO: Should this be deserializable?
		protected override bool SupportsDeserialization => false;

		protected override PutWatchDescriptor NewDescriptor() => new PutWatchDescriptor(CallIsolatedValue);

		protected override object ExpectJson =>
			new
			{
				input = new
				{
					chain = new
					{
						inputs = new object[]
						{
							new
							{
								simple = new
								{
									simple = new
									{
										str = "val1",
										num = 23,
										obj = new
										{
											str = "val2"
										}
									}
								}
							},
							new
							{
								http = new
								{
									http = new
									{
										request = new
										{
											host = "localhost",
											port = 8080,
											method = "post",
											path = "/path.html",
											proxy = new
											{
												host = "proxy",
												port = 6000
											},
											scheme = "https",
											auth = new
											{
												basic = new
												{
													username = "Username123",
													password = "Password123"
												}
											},
											body = "{\"query\" : {\"range\": {\"@timestamp\" : {\"from\": \"{{ctx.trigger.triggered_time}}||-5m\",\"to\": \"{{ctx.trigger.triggered_time}}\"}}}}",
											headers = new
											{
												header1 = "value1"
											},
											@params = new
											{
												lat = "52.374031",
												lon = "4.88969",
												appid = "appid"
											}
										},
										response_content_type = "text"
									}
								}
							},
							new
							{
								search = new
								{
									search = new
									{
										request = new
										{
											indices = new[] { "project" },
											body = new
											{
												size = 0,
												aggs = new
												{
													top_project_tags = new
													{
														terms = new
														{
															field = "tags.name"
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}

				},
				condition = new
				{
					array_compare = new JObject
					{
						{ "ctx.payload.search.aggregations.top_project_tags.buckets", new JObject
							{
								{ "path", "doc_count" },
								{ "gte", new JObject { { "value", 1 } } }
							}
						}
					}
				},
				trigger = new
				{
					schedule = new
					{
						weekly = new[]
						{
							new { on = new[] { "monday" }, at = new[] { "noon" } },
							new { on = new[] { "friday" }, at = new[] { "17:00" } }
						}
					}
				},
				actions = new
				{
					reminder_email = new
					{
						email = new
						{
							to = new[] { "me@example.com" },
							subject = "Something's strange in the neighbourhood",
							body = new
							{
								text = "Dear {{ctx.payload.name}}, by the time you read these lines, I'll be gone"
							}
						}
					}
				}
			};

		protected override Func<PutWatchDescriptor, IPutWatchRequest> Fluent => p => p
			.Input(i => i
				.Chain(c => c
					.Input("simple", ci => ci
						.Simple(s => s
							.Add("str", "val1")
							.Add("num", 23)
							.Add("obj", new { str = "val2" })
						)
					)
					.Input("http", ci => ci
						.Http(h => h
							.Request(r => r
								.Host("localhost")
								.Port(8080)
								.Method(HttpInputMethod.Post)
								.Path("/path.html")
								.Proxy(pr => pr
									.Host("proxy")
									.Port(6000)
								)
								.Scheme(ConnectionScheme.Https)
								.Authentication(a => a
									.Basic(b => b
										.Username("Username123")
										.Password("Password123")
									)
								)
								.Body("{\"query\" : {\"range\": {\"@timestamp\" : {\"from\": \"{{ctx.trigger.triggered_time}}||-5m\",\"to\": \"{{ctx.trigger.triggered_time}}\"}}}}")
								.Headers(he => he
									.Add("header1", "value1")
								)
								.Params(pa => pa
									.Add("lat", "52.374031")
									.Add("lon", "4.88969")
									.Add("appid", "appid")
								)
							)
							.ResponseContentType(ResponseContentType.Text)
						)
					)
					.Input("search", ci => ci
						.Search(s => s
							.Request(si => si
								.Indices<Project>()
								.Body<Project>(b => b
									.Size(0)
									.Aggregations(a => a
										.Terms("top_project_tags", ta => ta
											.Field(f => f.Tags.First().Name)
										)
									)
								)
							)
						)
					)
				)
			)
			.Condition(co => co
				.ArrayCompare(ac => ac
					.GreaterThanOrEqualTo("ctx.payload.search.aggregations.top_project_tags.buckets", "doc_count", 1)
				)
			)
			.Trigger(t => t
				.Schedule(s => s
					.Weekly(w => w
						.Add(ti => ti
							.On(Day.Monday)
							.At("noon")
						)
						.Add(ti => ti
							.On(Day.Friday)
							.At("17:00")
						)
					)
				)
			)
			.Actions(a => a
				.Add("reminder_email", new EmailAction
					{
						To = new [] { "me@example.com" },
						Subject = "Something's strange in the neighbourhood",
						Body = new EmailBody
						{
							Text = "Dear {{ctx.payload.name}}, by the time you read these lines, I'll be gone"
						}
					}
				)
			);

		protected override PutWatchRequest Initializer =>
			new PutWatchRequest(CallIsolatedValue)
			{
				Input = new ChainInput
				{
					Inputs = new Dictionary<string, InputContainer>
					{
						{
							"simple", new SimpleInput
							{
								{ "str", "val1" },
								{ "num", 23 },
								{ "obj", new { str = "val2" } }
							}
						},
						{
							"http", new HttpInput
							{
								Request = new HttpInputRequest
								{
									Host = "localhost",
									Port = 8080,
									Method = HttpInputMethod.Post,
									Path = "/path.html",
									Proxy = new HttpInputProxy
									{
										Host = "proxy",
										Port = 6000
									},
									Scheme = ConnectionScheme.Https,
									Authentication = new HttpInputAuthentication
									{
										Basic = new HttpInputBasicAuthentication
										{
											Username = "Username123",
											Password = "Password123"
										}
									},
									Body = "{\"query\" : {\"range\": {\"@timestamp\" : {\"from\": \"{{ctx.trigger.triggered_time}}||-5m\",\"to\": \"{{ctx.trigger.triggered_time}}\"}}}}",
									Headers = new Dictionary<string, string>
									{
										{ "header1", "value1" }
									},
									Params = new Dictionary<string, string>
									{
										{ "lat", "52.374031" },
										{ "lon", "4.88969" },
										{ "appid", "appid" },
									}
								},
								ResponseContentType = ResponseContentType.Text
							}
						},
						{
							"search", new SearchInput
							{
								Request = new SearchInputRequest
								{
									Indices = new IndexName[] { typeof(Project) },
									Body = new SearchRequest<Project>
									{
										Size = 0,
										Aggregations = new TermsAggregation("top_project_tags")
										{
											Field = Infer.Field<Project>(p => p.Tags.First().Name)
										}
									}
								}
							}
						}
					}
				},
				Condition = new GreaterThanOrEqualArrayCondition("ctx.payload.search.aggregations.top_project_tags.buckets", "doc_count", 1),
				Trigger = new ScheduleContainer
				{
					Weekly = new WeeklySchedule
					{
						new TimeOfWeek(Day.Monday, "noon"),
						new TimeOfWeek(Day.Friday, "17:00"),
					}
				},
				Actions = new Dictionary<string, IAction>
				{
					{
						"reminder_email", new EmailAction
						{
							To = new [] { "me@example.com" },
							Subject = "Something's strange in the neighbourhood",
							Body = new EmailBody
							{
								Text = "Dear {{ctx.payload.name}}, by the time you read these lines, I'll be gone"
							}
						}
					}
				}
			};

		protected override void ExpectResponse(IPutWatchResponse response)
		{
			response.Created.Should().BeTrue();
			response.Version.Should().Be(1);
			response.Id.Should().Be(new Id(CallIsolatedValue));
		}
	}
}
