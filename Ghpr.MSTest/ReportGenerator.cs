using Ghpr.Core;

namespace Ghpr.MSTest
{
    public static class ReportGenerator
    {
        public static void GenerateReport(string path)
        {
            var reporter = new Reporter();
            var reader = new TrxReader(path);
            reader.GenerateReport(reporter);
        }
    }
}