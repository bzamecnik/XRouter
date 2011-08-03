using System;
using System.Runtime.Serialization;
using System.Xml.XPath;

namespace XRouter.Common.Xrm
{
    [DataContract]
    public class XrmUri
    {
        [DataMember]
        public string XPath { get; set; }

        public XrmUri()
        {
            XPath = string.Empty;
        }

        public XrmUri(string xpath)
        {
            XPath = xpath;
        }

        public XrmUri(Uri uri)
        {
            XPath = uri.AbsolutePath;
        }

        public static bool IsXPathValid(string xpath)
        {
            try {
                XPathExpression.Compile(xpath);
                return true;
            } catch {
                return false;
            }
        }
    }
}
