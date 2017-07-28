using System;
using System.Linq;

namespace Ghpr.MSTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!args.Any())
            {
                throw new Exception("No path specified!");
            }
            var path = args[0];
            ReportGenerator.GenerateReport(path);
        }
    }
}
