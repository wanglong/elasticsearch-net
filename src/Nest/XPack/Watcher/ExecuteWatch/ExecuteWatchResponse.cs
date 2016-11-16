using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nest
{
	public interface IExecuteWatchResponse : IResponse
	{
		[JsonProperty("_id")]
		Id Id { get; set; }

		[JsonProperty("watch_record")]
		WatchRecord WatchRecord { get; set; }
	}

	public class ExecuteWatchResponse : ResponseBase, IExecuteWatchResponse
	{
		public Id Id { get; set; }

		public WatchRecord WatchRecord { get; set; }
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum ActionExecutionState
	{
		[EnumMember(Value = "awaits_execution")]
		AwaitsExecution,
		[EnumMember(Value = "checking")]
		Checking,
		[EnumMember(Value = "execution_not_needed")]
		ExecutionNotNeeded,
		[EnumMember(Value = "throttled")]
		Throttled,
		[EnumMember(Value = "executed")]
		Executed,
		[EnumMember(Value = "failed")]
		Failed,
		[EnumMember(Value = "deleted_while_queued")]
		DeletedWhileQueued
	}

	public class WatchRecord
	{
		[JsonProperty("watch_id")]
		public Id WatchId { get; set; }

		[JsonProperty("messages")]
		public IEnumerable<string> Messages { get; set; }

		[JsonProperty("state")]
		public ActionExecutionState? State { get; set; }

		[JsonProperty("trigger_event")]
		public TriggerEventResult TriggerEvent { get; set; }

		[JsonProperty("condition")]
		public ConditionContainer Condition { get; set; }

		[JsonProperty("input")]
		public InputContainer Input { get; set; }

		[JsonProperty("metadata")]
		public IDictionary<string, object> Metadata { get; set; }

		[JsonProperty("result")]
		public ExecutionResult Result { get; set; }
	}

	[JsonObject]
	public class TriggerEventResult
	{
		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("triggered_time")]
		public DateTime? TriggeredTime { get; set; }

		[JsonProperty("manual")]
		public ITriggerEventContainer Manual { get; set; }
	}

	public class ExecutionResult
	{
		[JsonProperty("execution_time")]
		public DateTime? ExecutionTime { get; set; }

		[JsonProperty("execution_duration")]
		public int? ExecutionDuration { get; set; }

		[JsonProperty("input")]
		public ExecutionResultInput Input { get; set; }

		[JsonProperty("condition")]
		public ExecutionResultCondition Condition { get; set; }

		[JsonProperty("actions")]
		public List<ExecutionResultAction> Actions { get; set; }
	}

	[JsonObject]
	public class ExecutionResultInput
	{
		[JsonProperty("type")]
		public InputType Type { get; set; }

		[JsonProperty("status")]
		public Status Status { get; set; }

		[JsonProperty("payload")]
		public Dictionary<string, object> Payload { get; set; }
	}

	[JsonObject]
	public class ExecutionResultCondition
	{
		[JsonProperty("type")]
		public ConditionType Type { get; set; }

		[JsonProperty("status")]
		public Status Status { get; set; }

		[JsonProperty("met")]
		public bool? Met { get; set; }
	}

	[JsonObject]
	public class ExecutionResultAction
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("type")]
		public ActionType Type { get; set; }

		[JsonProperty("status")]
		public Status Status { get; set; }

		//TODO: Add other actions here. Should these be coerced into an IActionResult common result?
		[JsonProperty("email")]
		public EmailActionResult Email { get; set; }

		[JsonProperty("index")]
		public IndexActionResult Index { get; set; }

		[JsonProperty("webhook")]
		public WebhookActionResult Webhook { get; set; }

		[JsonProperty("logging")]
		public LoggingActionResult Logging { get; set; }

		[JsonProperty("reason")]
		public string Reason { get; set; }
	}

	[JsonObject]
	public class EmailActionResult
	{
		[JsonProperty("reason")]
		public string Reason { get; set; }

		[JsonProperty("account")]
		public string Account { get; set; }

		[JsonProperty("message")]
		public EmailResult Message { get; set; }
	}

	[JsonObject]
	public class IndexActionResult
	{
		[JsonProperty("id")]
		public Id Id { get; set; }

		[JsonProperty("response")]
		public IndexActionResultIndexResponse Response { get; set; }
	}

	[JsonObject]
	public class IndexActionResultIndexResponse
	{
		[JsonProperty("index")]
		public IndexName Index { get; set; }

		[JsonProperty("type")]
		public TypeName Type { get; set; }

		[JsonProperty("version")]
		public int Version { get; set; }

		[JsonProperty("created")]
		public bool? Created { get; set; }

		[JsonProperty("result")]
		public Result Result { get; set; }

		[JsonProperty("id")]
		public Id Id { get; set; }
	}

	public class WebhookActionResult
	{
		[JsonProperty("request")]
		public HttpInputRequestResult Request { get; set; }

		[JsonProperty("response")]
		public HttpInputResponseResult Response { get; set; }
	}

	[JsonObject]
	[JsonConverter(typeof(ReadAsTypeJsonConverter<HttpInputRequestResult>))]
	public class HttpInputRequestResult : HttpInputRequest
	{
	}

	public class HttpInputResponseResult
	{
		[JsonProperty("status")]
		public int StatusCode { get; set; }

		[JsonProperty("headers")]
		public IDictionary<string, string[]> Headers { get; set; }

		[JsonProperty("body")]
		public string Body { get; set; }
	}

	[JsonObject]
	public class LoggingActionResult
	{
		[JsonProperty("logged_text")]
		public string LoggedText { get; set; }
	}
}
