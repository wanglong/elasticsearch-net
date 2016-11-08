using Newtonsoft.Json;

namespace Nest
{
	[JsonObject]
	public interface ILoggingAction : IAction
	{
		[JsonProperty("text")]
		string Text { get; set; }

		[JsonProperty("category")]
		string Category { get; set; }

		[JsonProperty("level")]
		LogLevel? Level { get; set; }
	}

	public class LoggingAction : ActionBase, ILoggingAction
	{
		public override ActionType ActionType => ActionType.Logging;
		public string Text { get; set; }
		public string Category { get; set; }
		public LogLevel? Level { get; set; }
	}
}
