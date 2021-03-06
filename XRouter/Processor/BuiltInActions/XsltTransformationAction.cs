﻿using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using ObjectConfigurator;
using XRouter.Common;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Xrm;
using System.IO;

namespace XRouter.Processor.BuiltInActions
{
    /// <summary>
    /// Message flow action which transforms a message using a XSLT into
    /// another message.
    /// </summary>
    /// <remarks>
    /// The XSL transform is obtained from the XML resource manager.
    /// </remarks>
    [ActionPlugin("XSLT transformer", "Transform a specified message with a XSL transformation.")]
    public class XsltTransformationAction : IActionPlugin
    {
        private IProcessorServiceForAction ProcessorService { get; set; }

        #region Configuration
        [ConfigurationItem("XSLT", "XRM URI of the XSLT (XPath for selecting the XSLT from XRM)",
            "//item[@name='xslt']")]
        private XrmUri xlstUri;

        [ConfigurationItem("Input message", "XPath for selecting the input message from a token",
            "token/messages/message[@name='input']/*[1]")]
        private TokenSelection inputMessageSelection;

        [ConfigurationItem("Output message name", null, "output")]
        private string outputMessageName;

        [ConfigurationItem("Is XSLT trusted",
            "Allows embedded script blocks and the XSLT document() function. " +
            "These are optional features which can be exploited by a malicious user. " +
            "Allow it only if the XSLT comes from a trusted source.", false)]
        private bool isXsltTrusted;
        #endregion

        private XslCompiledTransform xslTransform;

        public void Initialize(IProcessorServiceForAction processorService)
        {
            ProcessorService = processorService;

            XDocument xsltDocument = processorService.GetXmlResource(xlstUri);
            XmlReader xsltReader = xsltDocument.CreateReader();
            XsltSettings xsltSettings = XsltSettings.Default;
            if (isXsltTrusted)
            {
                xsltSettings = XsltSettings.TrustedXslt;
            }
            XmlResolver resolver = new XrmXmlResolver(ProcessorService);

            xslTransform = new XslCompiledTransform();
            xslTransform.Load(xsltReader, xsltSettings, resolver);
        }

        public void Evaluate(Token token)
        {
            TraceLog.Info(string.Format("Entering XSL transformation of '{0}' to message '{1}'",
                inputMessageSelection.SelectionPattern, outputMessageName));

            XDocument inputMessage = inputMessageSelection.GetSelectedDocument(token);
            var reader = inputMessage.Root.CreateReader();

            XDocument outputMessage;
            if (xslTransform.OutputSettings.OutputMethod != XmlOutputMethod.Xml) {
                StringWriter writer = new StringWriter();
                XmlDocument xmlInputMessage = new XmlDocument();
                xmlInputMessage.Load(reader);
                xslTransform.Transform(xmlInputMessage, null, writer);
                string result = writer.ToString();
                outputMessage = new XDocument(new XElement(XName.Get("content")));
                outputMessage.Root.Value = result;
            } else {
                StringBuilder outputBuilder = new StringBuilder();
                var writer = XmlWriter.Create(outputBuilder);
                xslTransform.Transform(reader, writer);
                outputMessage = XDocument.Parse(outputBuilder.ToString());
            }

            ProcessorService.CreateMessage(token, outputMessageName, outputMessage);
        }

        public void Dispose()
        {
        }
    }
}
