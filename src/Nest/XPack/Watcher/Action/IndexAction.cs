using Newtonsoft.Json;

namespace Nest
{
	[JsonObject]
	public interface IIndexAction : IAction
	{
		[JsonProperty("index")]
		IndexName Index { get; set; }
		[JsonProperty("doc_type")]
		TypeName DocType { get; set; }
		[JsonProperty("execution_time_field")]
		Field ExecutionTimeField { get; set; }
		[JsonProperty("timeout")]
		Timeout Timeout { get; set; }
	}

	public class IndexAction : ActionBase, IIndexAction
	{
		public override ActionType ActionType => ActionType.Index;
		public IndexName Index { get; set; }
		public TypeName DocType { get; set; }
		public Field ExecutionTimeField { get; set; }
		public Timeout Timeout { get; set; }
	}
}
