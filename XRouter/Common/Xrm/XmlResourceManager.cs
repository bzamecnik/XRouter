using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XRouter.Common.Xrm
{
    /// <summary>
    /// XML resource manager (also abbreviated as XRM) can store and load XML
    /// resources (XDocument instances) within its storage. It provides a
    /// unified way for storing many XML resources. Its backend can be
    /// arbitrary (in-memory collection, file system, database, etc.).
    /// </summary>
    public class XmlResourceManager
    {
        // TODO: unused!
        private Func<ApplicationConfiguration> GetConfiguration { get; set; }

        private IXmlStorage storage;

        /// <summary>
        /// Creates a new instance of the XML resource manager and initializes
        /// it with a provided XML storage.
        /// </summary>
        /// <param name="storage"></param>
        public XmlResourceManager(IXmlStorage storage)
        {
            this.storage = storage;
        }

        /// <summary>
        /// Gets the contents of a XML resource specified by its XRM URI.
        /// </summary>
        /// <param name="resourceUri">XRM URI of the requested resource</param>
        /// <returns>requested XML resource or null the there is no such a
        /// resource corresponding to the provided XRM URI or the resource is
        /// empty</returns>
        public XDocument GetXmlResource(XrmUri resourceUri)
        {
            XDocument content = storage.LoadXml();
            XElement xContainer = content.XPathSelectElement(resourceUri.XPath);
            if (xContainer == null) {
                return null;
            }
            XElement xItem = xContainer.Elements().FirstOrDefault();
            if (xItem == null) {
                return null;
            }

            XDocument result = new XDocument();
            result.Add(xItem);
            return result;
        }

        /// <summary>
        /// Sets the contents of a XML resource specified by its 
        /// </summary>
        /// <param name="resourceUri">XRM URI of the resource to be stored
        /// </param>
        /// <param name="resourceContent">contents of the XML resource</param>
        public void SetXmlResource(XrmUri resourceUri, XDocument resourceContent)
        {
            throw new NotImplementedException();
        }
    }
}
