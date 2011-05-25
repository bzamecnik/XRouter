using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XRouter.Common.MessageFlowConfig
{
    public class TokenSelection
    {
        private string _selectionPattern;
        public string SelectionPattern {
            get { return _selectionPattern; }
            set {
                _selectionPattern = value;
                XPath = _selectionPattern;
            }
        }

        public string XPath { get; private set; }

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
            XElement result = token.Content.XDocument.XPathSelectElement(XPath);
            return result;
        }
    }
}
