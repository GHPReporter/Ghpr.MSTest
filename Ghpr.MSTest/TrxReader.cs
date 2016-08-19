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
        //private static string _outputFolder;
        //private static string _fileName;
        private static string _fullPath;
        private readonly XmlDocument _xml;

        public TrxReader(string fullPath)
        {
            _fullPath = fullPath;
            _xml = new XmlDocument();
        }

        public TrxReader LoadResult()
        {
            if (!File.Exists(_fullPath))
            {
                throw new FileNotFoundException("Can't find .trx file!", _fullPath);
            }
            _xml.Load(_fullPath);
            return this;
        }

        public string GetRunGuid()
        {
            var tr = _xml.GetElementsByTagName("TestRun")[0];
            var id = tr.Attributes?["id"].Value ?? Guid.NewGuid().ToString();
            return id;
        }

        public List<ITestRun> GetTestRuns()
        {
            var testRuns = new List<ITestRun>();

            var utrs = _xml.GetElementsByTagName("UnitTestResult");
            
            foreach (XmlNode utr in utrs)
            {
                var start = DateTime.Parse(utr.Attributes?["startTime"].Value);
                var finish = DateTime.Parse(utr.Attributes?["endTime"].Value);
                var testGuid = utr.Attributes?["id"].Value ?? Guid.NewGuid().ToString();
                var testInfo = new ItemInfo
                {
                    Start = start,
                    Finish = finish,
                    Guid = Guid.Parse(testGuid)
                };
                var testName = utr.Attributes?["testName"].Value;
                var testFullName = _xml.GetElementById(testGuid)?
                    .GetElementsByTagName("TestMethod")[0]
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