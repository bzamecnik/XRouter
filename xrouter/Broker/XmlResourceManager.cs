using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using System.Xml.Linq;
using XRouter.Common.Xrm;

namespace XRouter.Broker
{
    internal class XmlResourceManager
    {
        private Func<ApplicationConfiguration> GetConfiguration { get; set; }

        private PersistentStorage storage;

        internal XmlResourceManager(PersistentStorage storage, Func<ApplicationConfiguration> getConfiguration)
        {
            this.storage = storage;
            GetConfiguration = getConfiguration;
        }

        public XDocument GetXmlResource(XrmTarget resourceUri)
        {
            throw new NotImplementedException();
        }

        public void SetXmlResource(Uri resourceUri, XDocument resourceContent)
        {
            throw new NotImplementedException();
        }
    }
}
