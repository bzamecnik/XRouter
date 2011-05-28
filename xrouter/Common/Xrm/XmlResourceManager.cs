﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XRouter.Common.Xrm
{
    public class XmlResourceManager
    {
        private Func<ApplicationConfiguration> GetConfiguration { get; set; }

        private IXmlStorage storage;

        public XmlResourceManager(IXmlStorage storage)
        {
            this.storage = storage;
        }

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

        public void SetXmlResource(Uri resourceUri, XDocument resourceContent)
        {
            throw new NotImplementedException();
        }
    }
}
