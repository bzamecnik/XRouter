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

namespace XRouter.Processor.BuiltInActions
{
    public class XsltTransformationAction : IActionPlugin
    {
        private IProcessorServiceForAction processorService;

        #region Configuration
        public XElement XConfig { get; set; }

        private XrmUri xlstUri;
        private TokenSelection inputMessageSelection;
        private string outputMessageName;
        #endregion

        private XslCompiledTransform xslTransform;

        public void Initialize(IProcessorServiceForAction processorService)
        {
            this.processorService = processorService;

            #region Emulate reading configuration (will be automatic later)
            xlstUri = new XrmUri("//item[@name='" + XConfig.Attribute(XName.Get("xslt")).Value + "']");

            inputMessageSelection = new TokenSelection("token/messages/message[@name='" + XConfig.Attribute(XName.Get("input")).Value + "']/*[1]");
            outputMessageName = XConfig.Attribute(XName.Get("output")).Value;
            #endregion

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

        // TODO: Disposing
        public void Dispose()
        {
        }
    }
}
