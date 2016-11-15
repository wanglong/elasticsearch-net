using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nest
{
	public interface IEmailAttachments : IIsADictionary<string, IEmailAttachment> { }

	public class EmailAttachments : IsADictionaryBase<string, IEmailAttachment>, IEmailAttachments
	{
		public EmailAttachments()
		{
		}
	}

	public interface IEmailAttachment { }

	[JsonObject]
	public class HttpAttachment : IEmailAttachment
	{
		[JsonProperty("content_type")]
		public string ContentType { get; set; }

		[JsonProperty("inline")]
		public bool? Inline { get; set; }

		[JsonProperty("request")]
		public IHttpInputRequest Request { get; set; }
	}

	public class DataAttachment : IEmailAttachment
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
}
