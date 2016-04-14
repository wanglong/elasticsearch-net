using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Tests.Document.Multiple.Bulk;
using BenchmarkDotNet.Jobs;

namespace Tests
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var summary = BenchmarkRunner.Run<BulkApiTests>(new Config());
		}

		class Config : ManualConfig
		{
			public Config()
			{
				Add(Job.AllJits);
				Add(Job.Dnx);
				Add(Job.Default.With(Mode.SingleRun).WithLaunchCount(1).WithWarmupCount(1).WithTargetCount(1));
				Add(Job.Default.With(BenchmarkDotNet.Jobs.Framework.Host).With(Runtime.Host).With(Platform.Host));
			}
		}
	}
}
