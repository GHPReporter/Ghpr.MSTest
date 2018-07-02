using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Ghpr.Core.Common;

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

        public List<KeyValuePair<TestRunDto,TestOutputDto>> GetTestRuns()
        {
            var testRuns = new List<KeyValuePair<TestRunDto, TestOutputDto>>();
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
                var testGuid = utr.GetAttrVal("testId") ?? Guid.NewGuid().ToString();
                var testInfo = new ItemInfoDto
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

                var testOutputInfo = new SimpleItemInfoDto
                {
                    Date = finish,
                    ItemName = "Test output"
                };

                var testRun = new TestRunDto
                {
                    TestInfo = testInfo,
                    Name = testName,
                    FullName = testFullName,
                    Result = result,
                    Output = testOutputInfo,
                    TestMessage = msg,
                    TestStackTrace = sTrace
                };

                var testOutput = new TestOutputDto
                {
                    TestOutputInfo = testOutputInfo,
                    Output = output,
                    SuiteOutput = ""
                };
                
                testRuns.Add(new KeyValuePair<TestRunDto, TestOutputDto>(testRun, testOutput));
            }

            return testRuns;
        }
    }
}
