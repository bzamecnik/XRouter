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
        }

        public XrmUri(string xpath)
        {
            XPath = xpath;
        }
    }
}
