using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.Xml.Linq;

namespace XRouter.Prototype.CoreTypes
{
    class XmlReduction
    {
        public IEnumerable<XPathExpression> XPaths { get; private set; }

        public XmlReduction()
        {
            XPaths = new XPathExpression[0]; 
        }

        public XmlReduction(IEnumerable<XPathExpression> xpaths)
        {
            XPaths = xpaths;
        }

        public XDocument GetReducedXml(XDocument originalDocument)
        {
            return originalDocument;
        }
    }
}
