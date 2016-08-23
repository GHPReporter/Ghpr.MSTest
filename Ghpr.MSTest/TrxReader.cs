using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Ghpr.Core;
using Ghpr.Core.Common;
using Ghpr.Core.Interfaces;

namespace Ghpr.MSTest
{
    public class TrxReader
    {
        private readonly XmlDocument _xml;

        public TrxReader(string fullPath)
        {
            _xml = XmlExtensions.GetDoc(fullPath);
        }
        
        public string GetRunGuid()
        {
            return _xml.GetNode("TestRun").GetAttrVal("id");
        }

        public List<ITestRun> GetTestRuns()
        {
            var testRuns = new List<ITestRun>();
            var utrs = _xml.GetNodesList("UnitTestResult");
            var uts = _xml.GetNode("TestDefinitions")?.GetNodesList("UnitTest");

            if (utrs == null)
            {
                Console.WriteLine("No tests found!");
                return testRuns;
            }
            
            foreach (var utr in utrs)
            {
                var start = DateTime.Parse(utr.GetAttrVal("startTime"));
                var finish = DateTime.Parse(utr.GetAttrVal("endTime"));
                var duration = utr.GetAttrVal("duration");
                var testGuid = utr.GetAttrVal("testId") ?? Guid.NewGuid().ToString();
                var testInfo = new ItemInfo
                {
                    Start = start,
                    Finish = finish,
                    Guid = Guid.Parse(testGuid)
                };
                var testName = utr.GetAttrVal("testName");
                var ut = uts?.FirstOrDefault(node => (node.GetAttrVal("id") ?? "").Equals(testGuid));
                var tm = ut?.GetNode("TestMethod");
                var testFullName = (tm?.GetAttrVal("className") ?? "").Split(',')[0] + "." + testName;
                var result = utr.GetAttrVal("outcome");
                var output = utr.GetNode("Output")?.GetNode("StdOut")?.InnerText ?? "";
                var msg = utr.GetNode("Output")?.GetNode("ErrorInfo")?.GetNode("Message")?.InnerText ?? "";
                var sTrace = utr.GetNode("Output")?.GetNode("ErrorInfo")?.GetNode("StackTrace")?.InnerText ?? "";
                var testRun = new TestRun
                {
                    TestInfo = testInfo,
                    Name = testName,
                    FullName = testFullName,
                    Result = result,
                    Output = output,
                    TestMessage = msg,
                    TestStackTrace = sTrace,
                    TestDuration = TimeSpan.Parse(duration).TotalSeconds
                };
                
                testRuns.Add(testRun);
            }

            return testRuns;
        }

        public void GenerateReport(Reporter r)
        {
            var runGuid = GetRunGuid();
            var testRuns = GetTestRuns();
            r.GenerateFullReport(testRuns, runGuid);
        }
    }
}