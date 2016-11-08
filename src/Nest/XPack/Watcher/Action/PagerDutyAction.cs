using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nest
{
	[JsonObject]
	public interface IPagerDutyAction : IAction
	{
		[JsonProperty("account")]
		string Account { get; set; }

		[JsonProperty("description")]
		string Description { get; set; }

		[JsonProperty("event_type")]
		PagerDutyEventType? EventType { get; set; }

		[JsonProperty("incident_key")]
		string IncidentKey { get; set; }

		[JsonProperty("client")]
		string Client { get; set; }

		[JsonProperty("client_url")]
		string ClientUrl { get; set; }

		[JsonProperty("attach_payload")]
		bool? AttachPayload { get; set; }

		[JsonProperty("contexts")]
		IEnumerable<PagerDutyContext> Contexts { get; set; }
	}

	public class PagerDutyAction : ActionBase, IPagerDutyAction
	{
		public override ActionType ActionType => ActionType.PagerDuty;
		public string Account { get; set; }

		public string Description { get; set; }

		public PagerDutyEventType? EventType { get; set; }

		public string IncidentKey { get; set; }

		public string Client { get; set; }

		public string ClientUrl { get; set; }

		public bool? AttachPayload { get; set; }

		public IEnumerable<PagerDutyContext> Contexts { get; set; }
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum PagerDutyEventType
	{
		[EnumMember(Value = "trigger")]
		Trigger,

		[EnumMember(Value = "resolve")]
		Resolve,

		[EnumMember(Value = "acknowledge")]
		Acknowledge
	}

	[JsonObject]
	public class PagerDutyContext
	{
		[JsonConstructor]
		public PagerDutyContext(PagerDutyContextType type)
		{
			this.Type = type;
		}

		[JsonProperty("type")]
		public PagerDutyContextType Type { get; set; }

		[JsonProperty("href")]
		public string Href { get; set; }

		[JsonProperty("src")]
		public string Src { get; set; }
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum PagerDutyContextType
	{
		[EnumMember(Value = "link")]
		Link,

		[EnumMember(Value = "image")]
		Image
	}
}
