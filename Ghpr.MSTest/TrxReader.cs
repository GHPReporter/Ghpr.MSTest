using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Ghpr.Core;
using Ghpr.Core.Common;
using Ghpr.Core.Interfaces;

namespace Ghpr.MSTest
{
    public class TrxReader
    {
        private const string Ns = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";

        private static string _fullPath;
        private readonly XmlDocument _xml;
        private XmlNamespaceManager _nsm;

        public TrxReader(string fullPath)
        {
            _fullPath = fullPath;
            _xml = new XmlDocument();

            if (!File.Exists(_fullPath))
            {
                throw new FileNotFoundException("Can't find .trx file!", _fullPath);
            }
            _xml.Load(_fullPath);

            _nsm = new XmlNamespaceManager(_xml.NameTable);
            _nsm.AddNamespace("ns", Ns);
        }
        
        public string GetRunGuid()
        {
            var tr = _xml.SelectSingleNode("//ns:TestRun", _nsm);

            Console.WriteLine("tr: " + (tr != null));

            var id = tr?.Attributes?["id"].Value ?? Guid.NewGuid().ToString();

            Console.WriteLine("id = " + id);

            return id;
        }

        public List<ITestRun> GetTestRuns()
        {
            var testRuns = new List<ITestRun>();

            var utrs = _xml.SelectNodes("//ns:UnitTestResult", _nsm);

            if (utrs == null)
            {
                Console.WriteLine("No tests found!");
                return testRuns;
            }

            foreach (XmlNode utr in utrs)
            {
                var start = DateTime.Parse(utr.Attributes?["startTime"].Value);
                var finish = DateTime.Parse(utr.Attributes?["endTime"].Value);
                var testGuid = utr.Attributes?["id"]?.Value ?? Guid.NewGuid().ToString();
                var testInfo = new ItemInfo
                {
                    Start = start,
                    Finish = finish,
                    Guid = Guid.Parse(testGuid)
                };
                var testName = utr.Attributes?["testName"].Value;
                var testFullName = _xml.GetElementById(testGuid)?
                    .GetElementsByTagName("testmethod")[0]
                    .Attributes?["className"]
                    .Value
                    .Split(',')[0] + "." + testName;
                var result = utr.Attributes?["outcome"].Value;
                var testRun = new TestRun
                {
                    TestInfo = testInfo,
                    Name = testName,
                    FullName = testFullName,
                    Result = result
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