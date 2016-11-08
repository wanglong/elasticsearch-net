namespace Nest
{
	public interface IInlineScriptTransform : IInlineScript, IScriptTransform { }

	public class InlineScriptTransform : InlineScript, IInlineScriptTransform
	{
		public InlineScriptTransform(string script) : base(script)
		{
		}
	}

	public class InlineScriptTransformDescriptor : InlineScriptDescriptor, IInlineScriptTransform
	{
		public InlineScriptTransformDescriptor(string inline) : base(inline)
		{
		}

		public InlineScriptTransformDescriptor()
		{
		}
	}
}
