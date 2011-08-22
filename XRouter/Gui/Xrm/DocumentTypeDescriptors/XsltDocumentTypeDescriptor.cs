using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Xsl;
using XRouter.Processor;

namespace XRouter.Gui.Xrm.DocumentTypeDescriptors
{
    class XsltDocumentTypeDescriptor : XDocumentTypeDescriptor
    {
        public override string DocumentTypeName { get { return "Xslt"; } }

        public override XElement CreateDefaultRoot()
        {
            return XElement.Parse(@"<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='1.0'>

</xsl:stylesheet>", LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
        }

        public override bool IsValid(XDocument xDocument, out string errorDescription)
        {
            XmlReader xsltReader = xDocument.CreateReader();
            XsltSettings xsltSettings = XsltSettings.Default;
            XmlResolver resolver = null;

            XslCompiledTransform xslTransform = new XslCompiledTransform();
            try {
                xslTransform.Load(xsltReader, xsltSettings, resolver);
                errorDescription = null;
                return true;
            } catch (Exception ex) {
                errorDescription = ex.Message;
                return false;
            }
        }
    }
}
