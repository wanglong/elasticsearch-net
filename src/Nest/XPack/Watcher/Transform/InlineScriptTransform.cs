using Newtonsoft.Json;

namespace Nest
{
	[JsonObject(MemberSerialization.OptIn)]
	public interface IInlineScriptTransform : IScriptTransform
	{
		[JsonProperty("inline")]
		string Inline { get; set; }
	}

	public class InlineScriptTransform : ScriptTransformBase, IInlineScriptTransform
	{
		public InlineScriptTransform(string script)
		{
			this.Inline = script;
		}

		public string Inline { get; set; }
	}

	public class InlineScriptTransformDescriptor
		: ScriptTransformDescriptorBase<InlineScriptTransformDescriptor, IInlineScriptTransform>, IInlineScriptTransform
	{
		public InlineScriptTransformDescriptor(string inline)
		{
			this.Inline = inline;
		}

		public InlineScriptTransformDescriptor()
		{
		}

		public string Inline { get; set; }
	}
}
