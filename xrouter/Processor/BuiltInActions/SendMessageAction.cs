using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using XRouter.Common.MessageFlowConfig;
using System.Xml.Linq;
using ObjectConfigurator;
using System.Threading.Tasks;

namespace XRouter.Processor.BuiltInActions
{
    [ActionPlugin("Message sender", "Sends a message to a specified output endpoint.")]
    public class SendMessageAction : IActionPlugin
    {
        private IProcessorServiceForAction processorService;

        #region Configuration
        [ConfigurationItem("Taget gateway", null, "gateway")]
        private string targetGatewayName;

        [ConfigurationItem("Target adapter", null, "directoryAdapter")]
        private string targetAdapterName;

        [ConfigurationItem("Target endpoint", null, "output")]
        private string targetEndpointName;

        [ConfigurationItem("Message", null, "token/messages/message[@name='input']/*[1]")]
        private TokenSelection messageSelection;

        [ConfigurationItem("Metadata", null, "token/source-metadata/file-metadata")]
        private TokenSelection metadataSelection;

        [ConfigurationItem("Result message name", null, "")]
        private string resultMessageName;

        [ConfigurationItem("Timeout (in seconds)", null, 30)]
        private int timeoutInSeconds;
        #endregion

        private EndpointAddress targetEndpoint;

        public void Initialize(IProcessorServiceForAction processorService)
        {
            this.processorService = processorService;
            targetEndpoint = new EndpointAddress(targetGatewayName, targetAdapterName, targetEndpointName);
        }

        public void Evaluate(Token token)
        {
            TraceLog.Info(string.Format("Sending '{0}' to output endpoint '{1}'", messageSelection.SelectionPattern, targetEndpoint.ToString()));

            XDocument message =  messageSelection.GetSelectedDocument(token);
            XDocument metadata = metadataSelection.GetSelectedDocument(token);

            XDocument outputMessage = null;
            Task sendTask = Task.Factory.StartNew(delegate {
                outputMessage = processorService.SendMessage(targetEndpoint, message, metadata);
            });
            bool isFinished = sendTask.Wait(TimeSpan.FromSeconds(timeoutInSeconds));

            if (isFinished) {
                if (resultMessageName.Trim().Length > 0) {
                    processorService.CreateMessage(token.Guid, resultMessageName.Trim(), outputMessage);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
