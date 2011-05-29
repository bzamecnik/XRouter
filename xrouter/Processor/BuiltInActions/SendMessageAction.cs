using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using XRouter.Common.MessageFlowConfig;
using System.Xml.Linq;

namespace XRouter.Processor.BuiltInActions
{
    public class SendMessageAction : IActionPlugin
    {
        private IProcessorServiceForAction processorService;

        #region Configuration
        public XElement XConfig { get; set; }

        private string targetGatewayName;
        private string targetAdapternName;
        private string targetEndpointName;

        private TokenSelection messageSelection;
        private TokenSelection metadataSelection;
        private string resultMessageName;
        #endregion

        private EndpointAddress targetEndpoint;

        public void Initialize(IProcessorServiceForAction processorService)
        {
            this.processorService = processorService;

            #region Emulate reading configuration (will be automatic later)
            targetGatewayName = "gateway1";
            targetAdapternName = "directoryAdapter";
            targetEndpointName = XConfig.Value;

            messageSelection = new TokenSelection("token/messages/message[@name='result']/content");
            metadataSelection = new TokenSelection("token/source-metadata/file-metadata");
            resultMessageName = string.Empty;
            #endregion

            targetEndpoint = new EndpointAddress(targetGatewayName, targetAdapternName, targetEndpointName);
        }

        public void Evaluate(Token token)
        {
            TraceLog.Info(string.Format("Sending '{0}' to output endpoint '{1}'", messageSelection.SelectionPattern, targetEndpoint.ToString()));

            XDocument message =  messageSelection.GetSelectedDocument(token);
            XDocument metadata = metadataSelection.GetSelectedDocument(token);

            XDocument outputMessage = processorService.SendMessage(targetEndpoint, message, metadata);

            if (resultMessageName.Length > 0) {
                processorService.CreateMessage(token.Guid, resultMessageName, outputMessage);
            }
        }

        // TODO: Disposing
        public void Dispose()
        {
        }
    }
}
