using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGenerator.Overrides.Descriptors
{
	public class XpackWatcherStatsDescriptorOverrides : DescriptorOverridesBase
	{
		public override IEnumerable<string> SkipQueryStringParams => new[]
		{
			"metric" // Favor setting as part of the url instead
		};
	}
}
