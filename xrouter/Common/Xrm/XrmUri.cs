using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.Runtime.Serialization;

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
