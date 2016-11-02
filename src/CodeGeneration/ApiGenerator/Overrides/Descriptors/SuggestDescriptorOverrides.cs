using System.Collections.Generic;

namespace ApiGenerator.Overrides.Descriptors
{
	public class SuggestDescriptorOverrides : DescriptorOverridesBase
	{
		public override IEnumerable<string> SkipQueryStringParams => new[]
		{
			"source"
		};
	}
}