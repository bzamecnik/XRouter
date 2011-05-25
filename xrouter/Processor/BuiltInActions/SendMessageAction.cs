using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using XRouter.Common.MessageFlowConfig;
using System.Xml.Linq;

namespace XRouter.Processor.BuiltInActions
{
    class SendMessageAction : IActionPlugin
    {
        private IProcessorServiceForAction processorService;

        #region Configuration
        private string targetGatewayName;
        private string targetAdapternName;
        private string targetEndpointName;

        private TokenSelection messageSelection;
        private string resultMessageName;
        #endregion

        private EndpointAddress targetEndpoint;

        public void Initialize(IProcessorServiceForAction processorService)
        {
            this.processorService = processorService;

            #region Emulate reading configuration (will be automatic later)
            targetGatewayName = "gateway1";
            targetAdapternName = "adapter1";
            targetEndpointName = "outputDir1";

            messageSelection = new TokenSelection("token/messages/message[@name='input']");
            resultMessageName = string.Empty;
            #endregion

            targetEndpoint = new EndpointAddress(targetGatewayName, targetAdapternName, targetEndpointName);
        }

        public void Evaluate(Token token)
        {
            XDocument message =  messageSelection.GetSelectedDocument(token);

            XDocument outputMessage = processorService.SendMessage(targetEndpoint, message);

            if (resultMessageName.Length > 0) {
                processorService.CreateMessage(token.Guid, resultMessageName, outputMessage);
            }
        }

        public void Dispose()
        {
        }
    }
}
