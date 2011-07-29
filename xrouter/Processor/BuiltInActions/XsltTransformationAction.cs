using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Xrm;
using XRouter.Common;
using System.Xml.Xsl;
using System.Xml;
using ObjectConfigurator;
using System.IO;

namespace XRouter.Processor.BuiltInActions
{
    [ActionPlugin("Xslt transformer", "Does xslt tansformation of a specified message.")]
    public class XsltTransformationAction : IActionPlugin
    {
        private IProcessorServiceForAction ProcessorService { get; set; }

        #region Configuration
        [ConfigurationItem("Xslt", null, "//item[@name='xslt']")]
        private XrmUri xlstUri;

        [ConfigurationItem("Input message", null, "token/messages/message[@name='input']/*[1]")]
        private TokenSelection inputMessageSelection;

        [ConfigurationItem("Output message name", null, "output")]
        private string outputMessageName;

        [ConfigurationItem("Is xslt trusted", "Allows embedded script blocks and the XSLT document() function. These are optional features which can be exploited by malicious user. Allow it only if xslt comes from trusted source.", false)]
        private bool isXsltTrusted;
        #endregion

        private XslCompiledTransform xslTransform;

        public void Initialize(IProcessorServiceForAction processorService)
        {
            ProcessorService = processorService;

            XDocument xsltDocument = processorService.GetXmlResource(xlstUri);
            XmlReader xsltReader = xsltDocument.CreateReader();
            XsltSettings xsltSettings = XsltSettings.Default;
            if (isXsltTrusted) {
                xsltSettings = XsltSettings.TrustedXslt;
            }
            XmlResolver resolver = new XrmXmlResolver(ProcessorService);

            xslTransform = new XslCompiledTransform();
            xslTransform.Load(xsltReader, xsltSettings, resolver);
        }

        public void Evaluate(Token token)
        {
            TraceLog.Info(string.Format("Entering xsl transformation of '{0}' to message '{1}'", inputMessageSelection.SelectionPattern, outputMessageName));

            XDocument inputMessage = inputMessageSelection.GetSelectedDocument(token);
            var reader = inputMessage.Root.CreateReader();

            StringBuilder outputBuilder = new StringBuilder();
            var writer = XmlWriter.Create(outputBuilder);
            xslTransform.Transform(reader, writer);

            XDocument outputMessage = XDocument.Parse(outputBuilder.ToString());

            ProcessorService.CreateMessage(token.Guid, outputMessageName, outputMessage);
        }

        public void Dispose()
        {
        }
    }
}
