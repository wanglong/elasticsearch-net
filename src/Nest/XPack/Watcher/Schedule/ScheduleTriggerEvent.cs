using System;
using Newtonsoft.Json;

namespace Nest
{
	[JsonObject]
	[JsonConverter(typeof(ReadAsTypeJsonConverter<ScheduleTriggerEvent>))]
	public interface IScheduleTriggerEvent : ITriggerEvent
	{
		[JsonProperty("triggered_time")]
		Union<DateTime, string> TriggeredTime { get; set; }

		[JsonProperty("scheduled_time")]
		Union<DateTime, string> ScheduledTime { get; set; }
	}


	public class ScheduleTriggerEvent : TriggerEventBase, IScheduleTriggerEvent
	{
		public Union<DateTime,string> TriggeredTime { get; set; }

		public Union<DateTime, string> ScheduledTime { get; set; }

		internal override void WrapInContainer(ITriggerEventContainer container) => container.Schedule = this;
	}

	public class ScheduleTriggerEventDescriptor
		: DescriptorBase<ScheduleTriggerEventDescriptor, IScheduleTriggerEvent>, IScheduleTriggerEvent
	{
		Union<DateTime, string> IScheduleTriggerEvent.TriggeredTime { get; set; }
		Union<DateTime, string> IScheduleTriggerEvent.ScheduledTime { get; set; }

		public ScheduleTriggerEventDescriptor TriggeredTime(DateTime triggeredTime) =>
			Assign(a => a.TriggeredTime = triggeredTime);

		public ScheduleTriggerEventDescriptor TriggeredTime(string triggeredTime) =>
			Assign(a => a.TriggeredTime = triggeredTime);

		public ScheduleTriggerEventDescriptor ScheduledTime(DateTime scheduledTime) =>
			Assign(a => a.ScheduledTime = scheduledTime);

		public ScheduleTriggerEventDescriptor ScheduledTime(string scheduledTime) =>
			Assign(a => a.ScheduledTime = scheduledTime);
	}
}
