using Newtonsoft.Json;

namespace Nest
{
	[JsonObject]
	public interface ICondition {}

	public abstract class ConditionBase
	{
		public static implicit operator ConditionContainer(ConditionBase condition) => condition == null
			? null
			: new ConditionContainer(condition);

		internal abstract void WrapInContainer(IConditionContainer container);
	}
}
