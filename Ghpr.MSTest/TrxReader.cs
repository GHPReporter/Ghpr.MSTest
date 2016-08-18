using System.Collections.Generic;
using System.IO;
using System.Xml;
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

        public List<ITestRun> GetTestRuns()
        {
            var testRuns = new List<ITestRun>();

            return testRuns;
        }


    }
}