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
            if (pattern.Length == 0) {
                return true;
            }

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
            if (pattern.StartsWith("$") && (pattern.Length > 1)) {
                return string.Format("/token/messages/message[@name='{0}']/*[1]", EncodeXml(pattern.Substring(1)));
            }
            return pattern;
        }

        private static string EncodeXml(string raw)
        {
            return raw
                .Replace("&", "&amp;")
                .Replace("'", "&apos;")
                .Replace("\"", "&quot;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }

        private string GetXPath()
        {
            return CreateXPathFromPattern(SelectionPattern);
        }
    }
}
