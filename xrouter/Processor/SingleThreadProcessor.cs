using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using System.Collections.Concurrent;
using XRouter.Broker;
using XRouter.Processor.MessageFlowParts;
using XRouter.Common.MessageFlowConfig;

namespace XRouter.Processor
{
    class SingleThreadProcessor
    {
        private BlockingCollection<Token> tokensToProcess;

        private ProcessorService processorService;
        private ProcessorServiceForNode serviceForNode;

        private Dictionary<Guid, MessageFlow> messageFlowsByGuid = new Dictionary<Guid, MessageFlow>();

        public SingleThreadProcessor(BlockingCollection<Token> tokensToProcess, ProcessorService processorService, ProcessorServiceForNode serviceForNode)
        {
            this.tokensToProcess = tokensToProcess;
            this.serviceForNode = serviceForNode;
            this.processorService = processorService;
        }

        public void Run()
        {
            foreach (Token token in tokensToProcess.GetConsumingEnumerable()) {
                MessageFlow messageFlow = GetMessageFlowForToken(token);
                bool canContinue = messageFlow.DoStep(token);
                if (canContinue) {
                    tokensToProcess.Add(token);
                } else {
                    processorService.DecrementTokensCount();
                    TraceLog.Info("Processor finished token with GUID " + token.Guid);
                }
            }
        }

        private MessageFlow GetMessageFlowForToken(Token token)
        {
            Guid messageFlowGuid = token.MessageFlowState.MessageFlowGuid;

            if (messageFlowsByGuid.ContainsKey(messageFlowGuid)) {
                return messageFlowsByGuid[messageFlowGuid];
            } else {
                MessageFlowConfiguration messageFlowConfiguration = processorService.Configuration.GetMessageFlow(messageFlowGuid);

                MessageFlow messageFlow = new MessageFlow(messageFlowConfiguration, serviceForNode);
                messageFlowsByGuid.Add(messageFlowGuid, messageFlow);
                return messageFlow;
            }
        }
    }
}
