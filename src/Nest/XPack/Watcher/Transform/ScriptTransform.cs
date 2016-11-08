using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Nest
{
	[JsonObject]
	[JsonConverter(typeof(ReadAsTypeJsonConverter<ScriptTransform>))]
	public interface IScriptTransform : ITransform, IScript
	{
	}

	// TODO: Fix the inheritance structure here. FileScriptTransform, etc. should inherit from this
	public class ScriptTransform : TransformBase, IScriptTransform
	{
		public Dictionary<string, object> Params { get; set; }

		public string Lang { get; set; }

		internal override void WrapInContainer(ITransformContainer container) => container.Script = this;
	}

	public class ScriptTransformDescriptor : DescriptorBase<ScriptTransformDescriptor, IDescriptor>
	{
		public FileScriptTransformDescriptor File(string file) => new FileScriptTransformDescriptor(file);

		public IndexedScriptTransformDescriptor Indexed(string id) => new IndexedScriptTransformDescriptor(id);

		public InlineScriptTransformDescriptor Inline(string script) => new InlineScriptTransformDescriptor(script);
	}
}
