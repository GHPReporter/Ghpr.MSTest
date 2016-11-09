using Ghpr.Core;

namespace Ghpr.MSTest
{
    public static class ReportGenerator
    {
        public static void GenerateReport(string path)
        {
            var settings = new Settings();
            var reporter = new Reporter(settings);
            var reader = new TrxReader(path);
            reader.GenerateReport(reporter);
        }
    }
}