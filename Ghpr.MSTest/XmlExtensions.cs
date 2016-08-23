using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Ghpr.MSTest
{
    public static class XmlExtensions
    {
        public const string Ns = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";
        private static XmlDocument _xml;
        private static XmlNamespaceManager _nsm;
        
        public static XmlDocument GetDoc(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Can't find .trx file!", path);
            }
            _xml = new XmlDocument();
            _xml.Load(path);
            _nsm = new XmlNamespaceManager(_xml.NameTable);
            _nsm.AddNamespace("ns", Ns);
            return _xml;
        }

        public static XmlNodeList GetNodes(this XmlDocument xml, string nodeName)
        {
            return xml.SelectNodes($".//ns:{nodeName}", _nsm);
        }

        public static List<XmlNode> GetNodesList(this XmlDocument xml, string nodeName)
        {
            return xml.SelectNodes($".//ns:{nodeName}", _nsm)?.Cast<XmlNode>().ToList();
        }

        public static XmlNode GetNode(this XmlDocument xml, string nodeName)
        {
            return xml.SelectSingleNode($".//ns:{nodeName}", _nsm);
        }

        public static XmlNodeList GetNodes(this XmlNode node, string nodeName)
        {
            return node.SelectNodes($".//ns:{nodeName}", _nsm);
        }

        public static List<XmlNode> GetNodesList(this XmlNode node, string nodeName)
        {
            return node.SelectNodes($".//ns:{nodeName}", _nsm)?.Cast<XmlNode>().ToList();
        }

        public static XmlNode GetNode(this XmlNode node, string nodeName)
        {
            return node.SelectSingleNode($".//ns:{nodeName}", _nsm);
        }

        public static string GetAttrVal(this XmlNode node, string attr)
        {
            return node.Attributes?[attr]?.Value;
        }
    }
}