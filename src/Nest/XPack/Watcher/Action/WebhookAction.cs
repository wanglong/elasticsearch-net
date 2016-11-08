using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nest
{
	[JsonObject]
	[JsonConverter(typeof(ReadAsTypeJsonConverter<WebhookAction>))]
	public interface IWebhookAction : IAction, IHttpInputRequest
	{
	}

	public class WebhookAction : ActionBase, IWebhookAction
	{
		public override ActionType ActionType => ActionType.Webhook;

		public ConnectionScheme? Scheme { get; set; }

		public int Port { get; set; }

		public string Host { get; set; }

		public string Path { get; set; }

		public HttpInputMethod? Method { get; set; }

		public IDictionary<string, string> Headers { get; set; }

		public IDictionary<string, string> Params { get; set; }

		public string Url { get; set; }

		public IHttpInputAuthentication Authentication { get; set; }

		public IHttpInputProxy Proxy { get; set; }

		public Timeout ConnectionTimeout { get; set; }

		public Timeout ReadTimeout { get; set; }

		public string Body { get; set; }
	}
}
