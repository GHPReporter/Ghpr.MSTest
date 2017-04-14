using Ghpr.Core;
using Ghpr.Core.Enums;

namespace Ghpr.MSTest
{
    public static class ReportGenerator
    {
        private static readonly Reporter reporter;

        static ReportGenerator()
        {
            reporter = new Reporter(TestingFramework.MSTest);
        }

        public static void GenerateReport(string trxPath)
        {
            var reader = new TrxReader(trxPath);
            var runGuid = reader.GetRunGuid();
            var testRuns = reader.GetTestRuns();

            reporter.GenerateFullReport(testRuns, runGuid);
        }
    }
}