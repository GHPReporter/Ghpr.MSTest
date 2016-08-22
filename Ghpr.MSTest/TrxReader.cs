using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Ghpr.Core;
using Ghpr.Core.Common;
using Ghpr.Core.Interfaces;

namespace Ghpr.MSTest
{
    public class TrxReader
    {
        private const string Ns = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";
        private readonly XmlDocument _xml;
        private readonly XmlNamespaceManager _nsm;

        public TrxReader(string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("Can't find .trx file!", fullPath);
            }
            _xml = new XmlDocument();
            _xml.Load(fullPath);
            _nsm = new XmlNamespaceManager(_xml.NameTable);
            _nsm.AddNamespace("ns", Ns);
        }
        
        public string GetRunGuid()
        {
            var tr = _xml.SelectSingleNode(".//ns:TestRun", _nsm);
            var id = tr?.Attributes?["id"].Value ?? Guid.NewGuid().ToString();
            return id;
        }

        public List<ITestRun> GetTestRuns()
        {
            var testRuns = new List<ITestRun>();

            var utrs = _xml.SelectNodes(".//ns:UnitTestResult", _nsm);

            if (utrs == null)
            {
                Console.WriteLine("No tests found!");
                return testRuns;
            }

            var uts = _xml.SelectSingleNode(".//ns:TestDefinitions", _nsm)?
                .SelectNodes(".//ns:UnitTest", _nsm)?
                .Cast<XmlNode>().ToList();

            foreach (XmlNode utr in utrs)
            {
                var start = DateTime.Parse(utr.Attributes?["startTime"].Value);
                var finish = DateTime.Parse(utr.Attributes?["endTime"].Value);
                var duration = utr.Attributes?["duration"].Value;
                var testGuid = utr.Attributes?["testId"]?.Value ?? Guid.NewGuid().ToString();
                var testInfo = new ItemInfo
                {
                    Start = start,
                    Finish = finish,
                    Guid = Guid.Parse(testGuid)
                };
                var testName = utr.Attributes?["testName"].Value;
                var ut = uts?.FirstOrDefault(node => (node.Attributes?["id"]?.Value ?? "").Equals(testGuid));
                var tm = ut?.SelectSingleNode(".//ns:TestMethod", _nsm);
                var testFullName = (tm?.Attributes?["className"].Value ?? "").Split(',')[0] + "." + testName;
                var result = utr.Attributes?["outcome"].Value;
                var output = utr.SelectSingleNode(".//ns:Output", _nsm)?.SelectSingleNode(".//ns:StdOut", _nsm)?.InnerText ?? "";
                var msg = utr.SelectSingleNode(".//ns:Output", _nsm)?.SelectSingleNode(".//ns:ErrorInfo", _nsm)?.SelectSingleNode(".//ns:Message", _nsm)?.InnerText ?? "";
                var sTrace = utr.SelectSingleNode(".//ns:Output", _nsm)?.SelectSingleNode(".//ns:ErrorInfo", _nsm)?.SelectSingleNode(".//ns:StackTrace", _nsm)?.InnerText ?? "";
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