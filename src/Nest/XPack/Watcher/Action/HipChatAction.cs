using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nest
{
	[JsonObject]
	public interface IHipChatAction : IAction
	{
		[JsonProperty("account")]
		string Account { get; set; }

		[JsonProperty("message")]
		HipChatMessage Message { get; set; }
	}

	public class HipChatAction : ActionBase, IHipChatAction
	{
		public override ActionType ActionType => ActionType.HipChat;

		public string Account { get; set; }

		public HipChatMessage Message { get; set; }
	}

	[JsonObject]
	public class HipChatMessage
	{
		[JsonProperty("body")]
		public string Body { get; set; }

		[JsonProperty("format")]
		public HipChatMessageFormat? Format { get; set; }

		[JsonProperty("color")]
		public HipChatMessageColor? Color { get; set; }

		[JsonProperty("notify")]
		public bool? Notify { get; set; }

		[JsonProperty("from")]
		public string From { get; set; }

		[JsonProperty("room")]
		[JsonConverter(typeof(ReadSingleOrEnumerableJsonConverter<string>))]
		public IEnumerable<string> Room { get; set; }

		[JsonProperty("user")]
		[JsonConverter(typeof(ReadSingleOrEnumerableJsonConverter<string>))]
		public IEnumerable<string> User { get; set; }
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum HipChatMessageFormat
	{
		[EnumMember(Value = "html")]
		Html,

		[EnumMember(Value = "text")]
		Text
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum HipChatMessageColor
	{
		[EnumMember(Value = "gray")]
		Gray,

		[EnumMember(Value = "green")]
		Green,

		[EnumMember(Value = "purple")]
		Purple,

		[EnumMember(Value = "red")]
		Red,

		[EnumMember(Value = "yellow")]
		Yellow,
	}
}
