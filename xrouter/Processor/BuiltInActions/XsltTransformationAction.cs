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

namespace XRouter.Processor.BuiltInActions
{
    public class XsltTransformationAction : IActionPlugin
    {
        private IProcessorServiceForAction processorService;

        #region Configuration
        [ConfigurationItem("Xslt", null, "//item[@name='xslt']")]
        private XrmUri xlstUri;

        [ConfigurationItem("Input message", null, "token/messages/message[@name='input']/*[1]")]
        private TokenSelection inputMessageSelection;

        [ConfigurationItem("Output message name", null, "output")]
        private string outputMessageName;
        #endregion

        private XslCompiledTransform xslTransform;

        public void Initialize(IProcessorServiceForAction processorService)
        {
            this.processorService = processorService;

            XDocument xsltDocument = processorService.GetXmlResource(xlstUri);
            xslTransform = new XslCompiledTransform();
            xslTransform.Load(xsltDocument.CreateReader());
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

            processorService.CreateMessage(token.Guid, outputMessageName, outputMessage);
        }

        public void Dispose()
        {
        }
    }
}
