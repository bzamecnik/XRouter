using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Runtime.Serialization;

namespace XRouter.Common.MessageFlowConfig
{
    [DataContract]
    public class TokenSelection
    {
        [DataMember]
        public string SelectionPattern { get; set; }

        public TokenSelection()
        {
            SelectionPattern = string.Empty;
        }

        public TokenSelection(string selectionPattern)
        {
            SelectionPattern = selectionPattern;
        }

        public XDocument GetSelectedDocument(Token token)
        {
            XElement selectedElement = GetSelectedElement(token);
            XDocument result = new XDocument();
            result.Add(selectedElement);
            return result;
        }

        public XElement GetSelectedElement(Token token)
        {
            string xpath = GetXPath();
            XElement result = token.Content.XDocument.XPathSelectElement(xpath);
            return result;
        }

        private string GetXPath()
        {
            return SelectionPattern;
        }
    }
}
