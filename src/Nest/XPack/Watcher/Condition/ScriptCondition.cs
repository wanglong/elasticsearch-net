using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nest
{
	[JsonObject]
	[JsonConverter(typeof(ReadAsTypeJsonConverter<ScriptCondition>))]
	public interface IScriptCondition : ICondition
	{
		[JsonProperty("inline")]
		string Inline { get; set; }

		[JsonProperty("lang")]
		string Lang { get; set; }

		[JsonProperty("params")]
		IDictionary<string, object> Params { get; set; }

		[JsonProperty("file")]
		string File { get; set; }

		[JsonProperty("id")]
		string Id { get; set; }
	}

	public class ScriptCondition : ConditionBase, IScriptCondition
	{
		public string Inline { get; set; }
		public string Lang { get; set; }
		public IDictionary<string, object> Params { get; set; }
		public string File { get; set; }
		public string Id { get; set; }

		internal override void WrapInContainer(IConditionContainer container) => container.Script = this;
	}

	// TODO: Revisit to see if this can be unified with IScript et. al
	public class ScriptConditionDescriptor
		: DescriptorBase<ScriptConditionDescriptor, IScriptCondition>, IScriptCondition
	{
		string IScriptCondition.Inline { get; set; }
		string IScriptCondition.Lang { get; set; }
		IDictionary<string, object> IScriptCondition.Params { get; set; }
		string IScriptCondition.File { get; set; }
		string IScriptCondition.Id { get; set; }

		public ScriptConditionDescriptor Inline(string script) => Assign(a => a.Inline = script);

		public ScriptConditionDescriptor Lang(string lang) => Assign(a => a.Lang = lang);

		public ScriptConditionDescriptor Params(Func<FluentDictionary<string, object>, FluentDictionary<string, object>> paramsDictionary) =>
			Assign(a => a.Params = paramsDictionary(new FluentDictionary<string, object>()));

		public ScriptConditionDescriptor Params(Dictionary<string, object> paramsDictionary) =>
			Assign(a => a.Params = paramsDictionary);

		public ScriptConditionDescriptor File(string file) => Assign(a => a.File = file);

		public ScriptConditionDescriptor Id(string id) => Assign(a => a.Id = id);
	}
}
