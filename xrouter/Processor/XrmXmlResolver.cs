using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using XRouter.Common.Xrm;

namespace XRouter.Processor
{
    public class XrmXmlResolver : XmlUrlResolver
    {
        public Func<XrmUri, XDocument> XrmResourceProvider { get; private set; }

        public XrmXmlResolver(IProcessorServiceForAction processorService)
        {
            XrmResourceProvider = processorService.GetXmlResource;
        }

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
                return memoryStream;
            } else {
                //otherwise use the default behavior of the XmlUrlResolver class (resolve resources from source)
                return base.GetEntity(absoluteUri, role, ofObjectToReturn);
            }
        }
    }
}
