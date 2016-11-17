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
		Actions Actions { get; set; }

		[JsonProperty("metadata")]
		[JsonConverter(typeof(VerbatimDictionaryKeysJsonConverter))]
		IDictionary<string, object> Metadata { get; set; }

		[JsonProperty("throttle_period")]
		string ThrottlePeriod { get; set; }

		[JsonProperty("transform")]
		TransformContainer Transform { get; set; }
	}

	public partial class PutWatchRequest
	{
		public PutWatchRequest() { }

		public IDictionary<string, object> Metadata { get; set; }

		public TriggerContainer Trigger { get; set; }

		public InputContainer Input { get; set; }

		public string ThrottlePeriod { get; set; }

		public ConditionContainer Condition { get; set; }

		public TransformContainer Transform { get; set; }

		public Actions Actions { get; set; }
	}

	[DescriptorFor("XpackWatcherPutWatch")]
	public partial class PutWatchDescriptor
	{
		public PutWatchDescriptor() { }

		Actions IPutWatchRequest.Actions { get; set; }
		ConditionContainer IPutWatchRequest.Condition { get; set; }
		InputContainer IPutWatchRequest.Input { get; set; }
		IDictionary<string, object> IPutWatchRequest.Metadata { get; set; }
		string IPutWatchRequest.ThrottlePeriod { get; set; }
		TransformContainer IPutWatchRequest.Transform { get; set; }
		TriggerContainer IPutWatchRequest.Trigger { get; set; }

		public PutWatchDescriptor Actions(Func<ActionsDescriptor, IPromise<Actions>> actions) =>
			Assign(a => a.Actions = actions?.Invoke(new ActionsDescriptor())?.Value);

		public PutWatchDescriptor Condition(Func<ConditionDescriptor, ConditionContainer> selector) =>
			Assign(a => a.Condition = selector.InvokeOrDefault(new ConditionDescriptor()));

		public PutWatchDescriptor Input(Func<InputDescriptor, InputContainer> selector) =>
			Assign(a => a.Input = selector.InvokeOrDefault(new InputDescriptor()));

		public PutWatchDescriptor Metadata(Func<FluentDictionary<string, object>, FluentDictionary<string, object>> paramsDictionary) =>
			Assign(a => a.Metadata = paramsDictionary(new FluentDictionary<string, object>()));

		public PutWatchDescriptor Metadata(Dictionary<string, object> paramsDictionary) =>
			Assign(a => a.Metadata = paramsDictionary);

		public PutWatchDescriptor ThrottlePeriod(string throttlePeriod) => Assign(a => a.ThrottlePeriod = throttlePeriod);

		public PutWatchDescriptor Transform(Func<TransformDescriptor, TransformContainer> selector) =>
			Assign(a => a.Transform = selector.InvokeOrDefault(new TransformDescriptor()));

		public PutWatchDescriptor Trigger(Func<TriggerDescriptor, TriggerContainer> selector) =>
			Assign(a => a.Trigger = selector.InvokeOrDefault(new TriggerDescriptor()));
	}
}
