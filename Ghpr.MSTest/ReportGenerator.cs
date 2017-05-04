using Ghpr.Core;
using Ghpr.Core.Enums;

namespace Ghpr.MSTest
{
    public static class ReportGenerator
    {
        public static void GenerateReport(string trxPath)
        {
            var reader = new TrxReader(trxPath);
            var runGuid = reader.GetRunGuid();
            var testRuns = reader.GetTestRuns();
            var reporter = new Reporter(TestingFramework.MSTest);
            reporter.GenerateFullReport(testRuns, runGuid);
        }
    }
}