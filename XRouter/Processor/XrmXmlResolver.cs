using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using XRouter.Common.Xrm;
using XRouter.Common.MessageFlowConfig;

namespace XRouter.Processor
{
    /// <summary>
    /// Resolves external XML resources referenced by a URI using the XML
    /// resource manager. Works as a decorator to XmlUrlResolver which add
    /// a new scheme using XRM.
    /// </summary>
    public class XrmXmlResolver : XmlUrlResolver
    {
        /// <summary>
        /// Function which actually loads XML resources from XRM.
        /// </summary>
        public Func<XrmUri, XDocument> XrmResourceProvider { get; private set; }

        /// <summary>
        /// Creates a new instance of XrmXmlResolver which load XRM resources
        /// via a given processor.
        /// </summary>
        /// <param name="processorService"></param>
        public XrmXmlResolver(IProcessorServiceForAction processorService)
        {
            XrmResourceProvider = processorService.GetXmlResource;
        }

        /// <summary>
        /// Maps a URI to an object containing the actual resource.
        /// </summary>
        /// <remarks>
        /// When the scheme of the URI is xrm:// the resource is loaded from
        /// the XML resource manager, otherwise the default behavior of
        /// XmlUrlResolver is used.
        /// </remarks>
        /// <param name="absoluteUri">absolute URI of the resource</param>
        /// <param name="role">not used</param>
        /// <param name="ofObjectToReturn">type of object to return;
        /// System.IO.Stream or null</param>
        /// <returns>Stream containing the resource</returns>
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri == null) {
                throw new ArgumentNullException("absoluteUri");
            }

            if ((absoluteUri.Scheme == "xrm") && ((ofObjectToReturn == null) || (ofObjectToReturn == typeof(Stream)))) {
                XrmUri xrmUri = new XrmUri(absoluteUri);
                XDocument xdocument = XrmResourceProvider(xrmUri);
                if (xdocument == null) {
                    return null;
                }

                MemoryStream memoryStream = new MemoryStream();
                using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream)) {
                    xdocument.WriteTo(xmlWriter);
                }
                memoryStream.Position = 0;
                return memoryStream;
            } else {
                //otherwise use the default behavior of the XmlUrlResolver class (resolve resources from source)
                return base.GetEntity(absoluteUri, role, ofObjectToReturn);
            }
        }
    }
}
