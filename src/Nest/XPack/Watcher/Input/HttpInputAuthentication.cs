using System;
using Newtonsoft.Json;

namespace Nest
{
	[JsonObject]
	public interface IHttpInputAuthentication
	{
		[JsonProperty("basic")]
		IHttpInputBasicAuthentication Basic { get; set; }
	}

	public class HttpInputAuthentication : IHttpInputAuthentication
	{
		public IHttpInputBasicAuthentication Basic { get; set; }
	}

	public class HttpInputAuthenticationDescriptor
		: DescriptorBase<HttpInputAuthenticationDescriptor, IHttpInputAuthentication>, IHttpInputAuthentication
	{
		IHttpInputBasicAuthentication IHttpInputAuthentication.Basic { get; set; }

		public HttpInputAuthenticationDescriptor Basic(Func<HttpInputBasicAuthenticationDescriptor, IHttpInputBasicAuthentication> selector) =>
			Assign(a => a.Basic = selector.Invoke(new HttpInputBasicAuthenticationDescriptor()));
	}

	[JsonObject]
	public interface IHttpInputBasicAuthentication
	{
		[JsonProperty("username")]
		string Username { get; set; }

		[JsonProperty("password")]
		string Password { get; set; }
	}

	[JsonObject]
	public class HttpInputBasicAuthentication : IHttpInputBasicAuthentication
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class HttpInputBasicAuthenticationDescriptor
		: DescriptorBase<HttpInputBasicAuthenticationDescriptor, IHttpInputBasicAuthentication>, IHttpInputBasicAuthentication
	{
		string IHttpInputBasicAuthentication.Username { get; set; }
		string IHttpInputBasicAuthentication.Password { get; set; }

		public HttpInputBasicAuthenticationDescriptor Username(string username) =>
			Assign(a => a.Username = username);

		public HttpInputBasicAuthenticationDescriptor Password(string password) =>
			Assign(a => a.Password = password);
	}
}
