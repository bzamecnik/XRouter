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

        public bool IsEmpty { get { return SelectionPattern.Trim().Length == 0; } }

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
            if (IsEmpty) {
                return null;
            }

            XElement selectedElement = GetSelectedElement(token);
            XDocument result = new XDocument();
            result.Add(selectedElement);
            return result;
        }

        public XElement GetSelectedElement(Token token)
        {
            if (IsEmpty) {
                return null;
            }

            string xpath = GetXPath();
            XElement result = token.Content.XDocument.XPathSelectElement(xpath);
            return result;
        }

        public static bool IsPatternValid(string pattern)
        {
            string xpath = CreateXPathFromPattern(pattern);
            try {
                XPathExpression.Compile(xpath);
                return true;
            } catch {
                return false;
            }
        }

        public static string CreateXPathFromPattern(string pattern)
        {
            return pattern;
        }

        private string GetXPath()
        {
            return SelectionPattern;
        }
    }
}
