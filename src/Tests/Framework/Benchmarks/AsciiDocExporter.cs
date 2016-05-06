using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Helpers;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace Tests.Framework.Benchmarks
{
	public class AsciiDocExporter : ExporterBase
	{
		protected override string FileExtension => "asciidoc";

		public static readonly IExporter Default = new AsciiDocExporter();

		private AsciiDocExporter()
		{
		}

		public override void ExportToLog(Summary summary, ILogger logger)
		{
			logger.WriteLine("....");
#if DOTNETCORE
			foreach (var infoLine in EnvironmentHelper.GetCurrentInfo().ToFormattedString("Host"))
#else
			foreach (var infoLine in EnvironmentInfo.GetCurrent().ToFormattedString("Host", true))
#endif
			{
				logger.WriteLineInfo(infoLine);
			}
			logger.WriteLine();

			PrintTable(summary.Table, logger);

#if DOTNETCORE
			var benchmarksWithTroubles = summary.Reports.Values.Where(r => !r.GetResultRuns().Any()).Select(r => r.Benchmark).ToList();
#else
			var benchmarksWithTroubles = summary.Reports.Where(r => !r.GetResultRuns().Any()).Select(r => r.Benchmark).ToList();
#endif
			if (benchmarksWithTroubles.Count > 0)
			{
				logger.WriteLine();
				logger.WriteLine("[WARNING]");
				logger.WriteLineError(".Benchmarks with issues");
				logger.WriteLine("====");
				foreach (var benchmarkWithTroubles in benchmarksWithTroubles)
					logger.WriteLineError("* " + benchmarkWithTroubles.ShortInfo);
				logger.WriteLine("====");
			}
		}

		private static void PrintTable(SummaryTable table, ILogger logger)
		{
			if (table.FullContent.Length == 0)
			{
				logger.WriteLine("[WARNING]");
				logger.WriteLine("====");
				logger.WriteLineError("There are no benchmarks found ");
				logger.WriteLine("====");
				logger.WriteLine();
				return;
			}

			table.PrintCommonColumns(logger);
			logger.Write("....");
			logger.WriteLine();

			logger.WriteLine("[options=\"header\"]");
			logger.WriteLine("|===");
			table.PrintLine(table.FullHeader, logger, "|", string.Empty);
			foreach (var line in table.FullContent)
				table.PrintLine(line, logger, "|", string.Empty);
			logger.WriteLine("|===");
		}
	}
}
