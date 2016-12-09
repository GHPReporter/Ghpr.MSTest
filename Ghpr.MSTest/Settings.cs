using System.Configuration;
using Ghpr.Core.Interfaces;

namespace Ghpr.MSTest
{
    public class Settings : IReporterSettings
    {
        public Settings()
        {
            OutputPath = ConfigurationManager.AppSettings["OutputPath"];
            TakeScreenshotAfterFail = false;
            Sprint = ConfigurationManager.AppSettings["Sprint"];
            RunName = ConfigurationManager.AppSettings["RunName"];
            RunGuid = ConfigurationManager.AppSettings["RunGuid"];
            RealTimeGeneration = false;
        }

        public string OutputPath { get; }
        public bool TakeScreenshotAfterFail { get; }
        public string Sprint { get; }
        public string RunName { get; }
        public string RunGuid { get; }
        public bool RealTimeGeneration { get; }
    }
}