using Ghpr.Core;

namespace Ghpr.MSTest
{
    public static class ReportGenerator
    {
        private static readonly Settings Settings;

        static ReportGenerator()
        {
            Settings = new Settings();
        }

        public static void GenerateReport(string trxPath)
        {
            var reporter = new Reporter(Settings);
            var reader = new TrxReader(trxPath);

            var runGuid = reader.GetRunGuid();
            var testRuns = reader.GetTestRuns();

            reporter.GenerateFullReport(testRuns, runGuid);
        }
    }
}