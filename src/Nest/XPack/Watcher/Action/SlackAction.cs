using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nest
{
	[JsonObject]
	public interface ISlackAction : IAction
	{
		[JsonProperty("account")]
		string Account { get; set; }

		[JsonProperty("message")]
		SlackMessage Message { get; set; }
	}

	[JsonObject]
	public class SlackMessage
	{
		[JsonProperty("from")]
		public string From { get; set; }

		[JsonProperty("to")]
		public IEnumerable<string> To { get; set; }

		[JsonProperty("icon")]
		public string Icon { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }

		[JsonProperty("attachments")]
		public IEnumerable<SlackAttachment> Attachments { get; set; }

		[JsonProperty("dynamic_attachments")]
		public SlackDynamicAttachment DynamicAttachments { get; set; }
	}

	public class SlackAction : ActionBase, ISlackAction
	{
		public override ActionType ActionType => ActionType.Slack;
		public string Account { get; set; }

		public SlackMessage Message { get; set; }
	}

	[JsonObject]
	public class SlackAttachmentField
	{
		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("value")]
		public string Value { get; set; }

		[JsonProperty("short")]
		public bool? Short { get; set; }
	}

	[JsonObject]
	public class SlackDynamicAttachment
	{
		[JsonProperty("list_path")]
		public string ListPath { get; set; }

		[JsonProperty("attachment_template")]
		public SlackAttachment AttachmentTemplate { get; set; }
	}

	[JsonObject]
	public class SlackAttachment
	{
		[JsonProperty("fallback")]
		public string Fallback { get; set; }

		[JsonProperty("color")]
		public string Color { get; set; }

		[JsonProperty("pretext")]
		public string Pretext { get; set; }

		[JsonProperty("author_name")]
		public string AuthorName { get; set; }

		[JsonProperty("author_link")]
		public string AuthorLink { get; set; }

		[JsonProperty("author_icon")]
		public string AuthorIcon { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("title_link")]
		public string TitleLink { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }

		[JsonProperty("fields")]
		public List<SlackAttachmentField> Fields { get; set; }

		[JsonProperty("image_url")]
		public string ImageUrl { get; set; }

		[JsonProperty("thumb_url")]
		public string ThumbUrl { get; set; }

		[JsonProperty("footer")]
		public string Footer { get; set; }

		[JsonProperty("footer_icon")]
		public string FooterIcon { get; set; }

		[JsonProperty("ts")]
		[JsonConverter(typeof(EpochDateTimeJsonConverter))]
		public DateTimeOffset? Ts { get; set; }
	}

	internal class EpochDateTimeJsonConverter : DateTimeConverterBase
	{
		private static readonly DateTimeOffset Epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var dateTimeOffset = value as DateTimeOffset?;

			if (dateTimeOffset == null)
			{
				var dateTime = value as DateTime?;
				if (dateTime == null)
				{
					writer.WriteNull();
					return;
				}

				var dateTimeDifference = (dateTime.Value - Epoch).TotalSeconds;
				writer.WriteValue(dateTimeDifference);
				return;
			}

			var dateTimeOffsetDifference = (dateTimeOffset.Value - Epoch).TotalSeconds;
			writer.WriteValue(dateTimeOffsetDifference);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.Float)
			{
				if (objectType == typeof(DateTimeOffset?) || objectType == typeof(DateTime?))
					return null;

				return objectType == typeof(DateTimeOffset)
					? default(DateTimeOffset)
					: default(DateTime);
			}

			var secondsSinceEpoch = (double)reader.Value;
			var dateTimeOffset = Epoch.Add(TimeSpan.FromSeconds(secondsSinceEpoch));

			return objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?)
				? dateTimeOffset
				: dateTimeOffset.DateTime;
		}
	}
}
