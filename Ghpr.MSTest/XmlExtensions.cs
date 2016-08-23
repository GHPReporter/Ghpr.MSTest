using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Ghpr.MSTest
{
    public static class XmlExtensions
    {
        public const string Ns = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";
        private static XmlNamespaceManager _nsm;
        
        public static XmlDocument GetDoc(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Can't find .trx file!", path);
            }
            var xml = new XmlDocument();
            xml.Load(path);
            _nsm = new XmlNamespaceManager(xml.NameTable);
            _nsm.AddNamespace("ns", Ns);
            return xml;
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