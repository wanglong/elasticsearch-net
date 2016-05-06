using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Running;
using Tests.Document.Multiple.Bulk;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Helpers;
using Tests.ClientConcepts.HighLevel.Caching;
using Tests.Framework;

namespace Tests
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var benchmarks = typeof(Program).Assembly().GetTypes()
				.Where(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public)
							 .Any(m => m.GetCustomAttributes(typeof(BenchmarkAttribute), false).Any()))
				.OrderBy(t => t.Namespace)
				.ThenBy(t => t.Name)
				.ToArray();
			var benchmarkSwitcher = new BenchmarkSwitcher(benchmarks);
			benchmarkSwitcher.Run(args);
		}


	}
}
