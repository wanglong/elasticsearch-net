using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nest
{
	public interface IActions : IIsADictionary<string, IAction> { }

	public class Actions : IsADictionaryBase<string, IAction>, IActions
	{
		public Actions()
		{
		}

		public Actions(IDictionary<string, IAction> actions) : base(actions)
		{
		}

		public static implicit operator Actions(ActionBase action)
		{
			if (action == null) return null;

			if (action.Name.IsNullOrEmpty())
				throw new ArgumentException($"{action.GetType().Name}.Name is not set!");

			var actions = new Dictionary<string, IAction>{{ action.Name, action }};
			return new Actions(actions);
		}
	}

	public class ActionsDescriptor : IsADictionaryDescriptorBase<ActionsDescriptor, IActions, string, IAction>
	{
		public ActionsDescriptor() : base(new Actions())
		{
		}

		public ActionsDescriptor Email(string name, Func<EmailActionDescriptor, IEmailAction> selector) =>
			Assign(name, selector.InvokeOrDefault(new EmailActionDescriptor(name)));

		public ActionsDescriptor HipChat(string name, Func<HipChatActionDescriptor, IHipChatAction> selector) =>
			Assign(name, selector.InvokeOrDefault(new HipChatActionDescriptor(name)));

		public ActionsDescriptor Index(string name, Func<IndexActionDescriptor, IIndexAction> selector) =>
			Assign(name, selector.InvokeOrDefault(new IndexActionDescriptor(name)));

		public ActionsDescriptor Logging(string name, Func<LoggingActionDescriptor, ILoggingAction> selector) =>
			Assign(name, selector.InvokeOrDefault(new LoggingActionDescriptor(name)));

		public ActionsDescriptor PagerDuty(string name, Func<PagerDutyActionDescriptor, IPagerDutyAction> selector) =>
			Assign(name, selector.InvokeOrDefault(new PagerDutyActionDescriptor(name)));

		public ActionsDescriptor Slack(string name, Func<SlackActionDescriptor, ISlackAction> selector) =>
			Assign(name, selector.InvokeOrDefault(new SlackActionDescriptor(name)));

		public ActionsDescriptor Webhook(string name, Func<WebhookActionDescriptor, IWebhookAction> selector) =>
			Assign(name, selector.InvokeOrDefault(new WebhookActionDescriptor(name)));
	}

	public partial interface IPutWatchRequest
	{
		[JsonProperty("trigger")]
		TriggerContainer Trigger { get; set; }

		[JsonProperty("input")]
		InputContainer Input { get; set; }

		[JsonProperty("condition")]
		ConditionContainer Condition { get; set; }

		[JsonProperty("actions")]
		[JsonConverter(typeof(ActionsJsonConverter))]
		IActions Actions { get; set; }

		[JsonProperty("meta")]
		[JsonConverter(typeof(VerbatimDictionaryKeysJsonConverter))]
		IDictionary<string, object> Meta { get; set; }

		[JsonProperty("throttle_period")]
		string ThrottlePeriod { get; set; }

		[JsonProperty("transform")]
		TransformContainer Transform { get; set; }
	}

	public partial class PutWatchRequest
	{
		public IDictionary<string, object> Meta { get; set; }

		public TriggerContainer Trigger { get; set; }

		public InputContainer Input { get; set; }

		public string ThrottlePeriod { get; set; }

		public ConditionContainer Condition { get; set; }

		public TransformContainer Transform { get; set; }

		public IActions Actions { get; set; }
	}

	[DescriptorFor("XpackWatcherPutWatch")]
	public partial class PutWatchDescriptor
	{
		public PutWatchDescriptor() { }

		IActions IPutWatchRequest.Actions { get; set; }
		ConditionContainer IPutWatchRequest.Condition { get; set; }
		InputContainer IPutWatchRequest.Input { get; set; }
		IDictionary<string, object> IPutWatchRequest.Meta { get; set; }
		string IPutWatchRequest.ThrottlePeriod { get; set; }
		TransformContainer IPutWatchRequest.Transform { get; set; }
		TriggerContainer IPutWatchRequest.Trigger { get; set; }

		public PutWatchDescriptor Actions(Func<ActionsDescriptor, IPromise<IActions>> actions) =>
			Assign(a => a.Actions = actions?.Invoke(new ActionsDescriptor())?.Value);

		public PutWatchDescriptor Condition(Func<ConditionDescriptor, ConditionContainer> selector) =>
			Assign(a => a.Condition = selector.InvokeOrDefault(new ConditionDescriptor()));

		public PutWatchDescriptor Input(Func<InputDescriptor, InputContainer> selector) =>
			Assign(a => a.Input = selector.InvokeOrDefault(new InputDescriptor()));

		public PutWatchDescriptor Meta(Func<FluentDictionary<string, object>, FluentDictionary<string, object>> paramsDictionary) =>
			Assign(a => a.Meta = paramsDictionary(new FluentDictionary<string, object>()));

		public PutWatchDescriptor Meta(Dictionary<string, object> paramsDictionary) =>
			Assign(a => a.Meta = paramsDictionary);

		public PutWatchDescriptor ThrottlePeriod(string throttlePeriod) => Assign(a => a.ThrottlePeriod = throttlePeriod);

		public PutWatchDescriptor Transform(Func<TransformDescriptor, TransformContainer> selector) =>
			Assign(a => a.Transform = selector.InvokeOrDefault(new TransformDescriptor()));

		public PutWatchDescriptor Trigger(Func<TriggerDescriptor, TriggerContainer> selector) =>
			Assign(a => a.Trigger = selector.InvokeOrDefault(new TriggerDescriptor()));
	}
}
