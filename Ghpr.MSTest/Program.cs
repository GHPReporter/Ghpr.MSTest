using System;
using System.Linq;
using Ghpr.Core;

namespace Ghpr.MSTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!args.Any()) throw new Exception("No path specified!");
            var path = args[0];
            var reporter = new Reporter();
            var reader = new TrxReader(path);

            reader.GenerateReport(reporter);

        }
    }
}
