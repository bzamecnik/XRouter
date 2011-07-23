using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.Xml.Linq;

namespace XRouter.Common
{
    /// <summary>
    /// Represents a filter which can extract interesting parts from a big XML.
    /// </summary>
    /// <remarks>
    /// Idented usage is for components which need only a small portion of a
    /// potentially big XML configuration. The rest can be saved from eg. a
    /// network transfer.
    /// </remarks>
    public class XmlReduction
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
