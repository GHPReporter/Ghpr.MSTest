// ReSharper disable InconsistentNaming
using System;
using Ghpr.Core;
using Ghpr.Core.Enums;

namespace Ghpr.MSTest.Utils
{
    public class GhprMSTestRunHelper
    {
        public static void CreateReportFromFile(string path)
        {
            var reporter = new Reporter(TestingFramework.MSTest);
            try
            {
                var reader = new TrxReader(path);
                var testRuns = reader.GetTestRuns();
                reporter.GenerateFullReport(testRuns);
            }
            catch (Exception ex)
            {
                var log = new Core.Utils.Log(reporter.Settings.OutputPath);
                log.Exception(ex, "Exception in CreateReportFromFile");
            }
        }
    }
}