using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.Xml.Linq;
using XRouter.Remoting;

namespace XRouter.Management
{
    public class RemotableXElement : IRemotelyCloneable
    {
        private static readonly string SerializationSeparator = "[[(-t^1g*_j/w++}]]";

        internal RemotableXDocument RemotableXDocument { get; private set; }

        public XElement XElement { get; private set; }

        public RemotableXElement(XElement xelement)
        {
            XElement = xelement;
            RemotableXDocument = new RemotableXDocument(XElement.Document);
        }

        public RemotableXElement(string serialized)
        {
            DeserializeClone(serialized);
        }

        public string SerializeClone()
        {
            var result = new StringBuilder();

            result.Append(RemotableXDocument.SerializeClone());
            result.Append(SerializationSeparator);
            
            string xpath = GetFullXPath(XElement);
            result.Append(xpath);

            return result.ToString();
        }

        public void DeserializeClone(string serialized)
        {
            string[] parts = serialized.Split(new string[] { SerializationSeparator }, StringSplitOptions.None);

            RemotableXDocument = new RemotableXDocument(parts[0]);

            string xpath = parts[1];
            XElement = RemotableXDocument.XDocument.XPathSelectElement(xpath);
        }

        private static string GetFullXPath(XElement element)
        {
            StringBuilder result = new StringBuilder();
            foreach (var e in element.AncestorsAndSelf().Reverse()) {
                int index = GetIndexOfChildElement(e);
                if (index == -1) {
                    result.Append(string.Format("/{0}", e.Name));
                } else {
                    result.Append(string.Format("/{0}[{1}]", e.Name, index));
                }
            }
            return result.ToString();
        }

        private static int GetIndexOfChildElement(XElement child)
        {
            if (child.Parent == null) {
                return -1;
            }

            XElement[] siblings = child.Parent.Elements(child.Name).ToArray();
            for (int i = 0; i < siblings.Length; i++) {
                if (child.Equals(siblings[i])) {
                    return i;
                }
            }
            return -1;
        }
    }
}
