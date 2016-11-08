using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Nest
{
	[JsonObject]
	public interface IEmailAction : IAction
	{
		[JsonProperty("account")]
		string Account { get; set; }
		[JsonProperty("from")]
		string From { get; set; }
		[JsonProperty("to")]
		IEnumerable<string> To { get; set; }
		[JsonProperty("cc")]
		IEnumerable<string> Cc { get; set; }
		[JsonProperty("bcc")]
		IEnumerable<string> Bcc { get; set; }
		[JsonProperty("reply_to")]
		IEnumerable<string> ReplyTo { get; set; }
		[JsonProperty("subject")]
		string Subject { get; set; }
		[JsonProperty("body")]
		EmailBody Body { get; set; }
		[JsonProperty("priority")]
		EmailPriority? Priority { get; set; }

		[JsonProperty("attachments")]
		IDictionary<string, EmailAttachmentBase> Attachments { get; set; }
	}

	public abstract class EmailAttachmentBase {}

	[JsonObject]
	public class HttpAttachment : EmailAttachmentBase
	{
		[JsonProperty("content_type")]
		public string ContentType { get; set; }

		[JsonProperty("inline")]
		public bool? Inline { get; set; }

		[JsonProperty("request")]
		public IHttpInputRequest Request { get; set; }
	}

	public class DataAttachment : EmailAttachmentBase
	{
		[JsonProperty("format")]
		public DataAttachmentFormat? Format { get; set; }
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum DataAttachmentFormat
	{
		[EnumMember(Value = "json")]
		Json,
		[EnumMember(Value = "yaml")]
		Yaml
	}


	[JsonObject]
	public class EmailBody
	{
		[JsonProperty("text")]
		public string Text { get; set; }
		[JsonProperty("html")]
		public string Html { get; set; }
	}

	public class EmailAction : ActionBase, IEmailAction
	{
		public override ActionType ActionType => ActionType.Email;

		public string Account { get; set; }

		public string From { get; set; }

		public IEnumerable<string> To { get; set; }

		public IEnumerable<string> Cc { get; set; }

		public IEnumerable<string> Bcc { get; set; }

		public IEnumerable<string> ReplyTo { get; set; }

		public string Subject { get; set; }

		public EmailBody Body { get; set; }

		public EmailPriority? Priority { get; set; }

		public IDictionary<string, EmailAttachmentBase> Attachments { get; set; }
	}

	public class EmailResult
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("sent_date")]
		public DateTime? SentDate { get; set; }

		[JsonProperty("from")]
		public string From { get; set; }

		[JsonProperty("to")]
		public IEnumerable<string> To { get; set; }

		[JsonProperty("cc")]
		public IEnumerable<string> Cc { get; set; }

		[JsonProperty("bcc")]
		public IEnumerable<string> Bcc { get; set; }

		[JsonProperty("reply_to")]
		public IEnumerable<string> ReplyTo { get; set; }

		[JsonProperty("subject")]
		public string Subject { get; set; }

		[JsonProperty("body")]
		public EmailBody Body { get; set; }

		[JsonProperty("priority")]
		public EmailPriority? Priority { get; set; }
	}
}
