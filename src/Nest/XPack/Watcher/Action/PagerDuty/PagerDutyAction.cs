using System;
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

		public PagerDutyAction(string name) : base(name)
		{
		}
	}

	public class PagerDutyActionDescriptor : ActionsDescriptorBase<PagerDutyActionDescriptor, IPagerDutyAction>, IPagerDutyAction
	{
		protected override ActionType ActionType => ActionType.PagerDuty;

		string IPagerDutyAction.Account { get; set; }
		string IPagerDutyAction.Description { get; set; }
		PagerDutyEventType? IPagerDutyAction.EventType { get; set; }
		string IPagerDutyAction.IncidentKey { get; set; }
		string IPagerDutyAction.Client { get; set; }
		string IPagerDutyAction.ClientUrl { get; set; }
		bool? IPagerDutyAction.AttachPayload { get; set; }
		IEnumerable<PagerDutyContext> IPagerDutyAction.Contexts { get; set; }

		public PagerDutyActionDescriptor(string name) : base(name)
		{
		}

		public PagerDutyActionDescriptor Account(string account) => Assign(a => a.Account = account);

		public PagerDutyActionDescriptor Description(string description) => Assign(a => a.Description = description);

		public PagerDutyActionDescriptor EventType(PagerDutyEventType eventType) => Assign(a => a.EventType = eventType);

		public PagerDutyActionDescriptor IncidentKey(string incidentKey) => Assign(a => a.IncidentKey = incidentKey);

		public PagerDutyActionDescriptor Client(string client) => Assign(a => a.Client = client);

		public PagerDutyActionDescriptor ClientUrl(string url) => Assign(a => a.ClientUrl = url);

		public PagerDutyActionDescriptor AttachPayload(bool attach = true) => Assign(a => a.AttachPayload = attach);

		public PagerDutyActionDescriptor Contexts(Func<PagerDutyContextsDescriptor, IPromise<IList<PagerDutyContext>>> selector) =>
			Assign(a => a.Contexts = selector?.Invoke(new PagerDutyContextsDescriptor())?.Value);
	}

	public class PagerDutyContextsDescriptor
	: DescriptorPromiseBase<PagerDutyContextsDescriptor, IList<PagerDutyContext>>
	{
		public PagerDutyContextsDescriptor() : base(new List<PagerDutyContext>()) { }

		public PagerDutyContextsDescriptor Context(PagerDutyContextType type, Func<PagerDutyContextDescriptor, IPagerDutyContext> selector) =>
			this.Assign(a => a.AddIfNotNull(selector?.Invoke(new PagerDutyContextDescriptor(type))));
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
	[JsonConverter(typeof(ReadAsTypeJsonConverter<PagerDutyContext>))]
	public interface IPagerDutyContext
	{
		[JsonProperty("type")]
		PagerDutyContextType Type { get; set; }

		[JsonProperty("href")]
		string Href { get; set; }

		[JsonProperty("src")]
		string Src { get; set; }
	}

	public class PagerDutyContext : IPagerDutyContext
	{
		public PagerDutyContext(PagerDutyContextType type)
		{
			this.Type = type;
		}

		internal PagerDutyContext() { }

		public PagerDutyContextType Type { get; set; }

		public string Href { get; set; }

		public string Src { get; set; }
	}

	public class PagerDutyContextDescriptor : DescriptorBase<PagerDutyContextDescriptor, IPagerDutyContext>, IPagerDutyContext
	{
		PagerDutyContextType IPagerDutyContext.Type { get; set; }
		string IPagerDutyContext.Href { get; set; }
		string IPagerDutyContext.Src { get; set; }

		public PagerDutyContextDescriptor(PagerDutyContextType type)
		{
			Self.Type = type;
		}

		public PagerDutyContextDescriptor Type(PagerDutyContextType type) => Assign(a => a.Type = type);

		public PagerDutyContextDescriptor Href(string href) => Assign(a => a.Href = href);

		public PagerDutyContextDescriptor Src(string src) => Assign(a => a.Src = src);
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
