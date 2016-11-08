using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nest
{
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
		IDictionary<string, IAction> Actions { get; set; }

		[JsonProperty("meta")]
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

		public IDictionary<string, IAction> Actions { get; set; }
	}

	[DescriptorFor("XpackWatcherPutWatch")]
	public partial class PutWatchDescriptor
	{
		public PutWatchDescriptor() { }

		IDictionary<string, IAction> IPutWatchRequest.Actions { get; set; }
		ConditionContainer IPutWatchRequest.Condition { get; set; }
		InputContainer IPutWatchRequest.Input { get; set; }
		IDictionary<string, object> IPutWatchRequest.Meta { get; set; }
		string IPutWatchRequest.ThrottlePeriod { get; set; }
		TransformContainer IPutWatchRequest.Transform { get; set; }
		TriggerContainer IPutWatchRequest.Trigger { get; set; }

		// TODO: Introduce an ActionsDescriptor that creates actions using action method names e.g. Email(Func<T,I>)
		public PutWatchDescriptor Actions(Func<FluentDictionary<string, IAction>, FluentDictionary<string, IAction>> actions) =>
			Assign(a => a.Actions = actions(new FluentDictionary<string, IAction>()));

		public PutWatchDescriptor Actions(Dictionary<string, IAction> actions) => Assign(a => a.Actions = actions);

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
