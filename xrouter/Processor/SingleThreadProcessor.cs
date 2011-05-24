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
        private volatile bool isStopped = false;

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
                if (isStopped) {
                    return;
                }
                MessageFlow messageFlow = GetMessageFlowForToken(token);
                bool canContinue = messageFlow.DoStep(token);
                if (canContinue) {
                    tokensToProcess.Add(token);
                }
            }
        }

        public void Stop()
        {
            isStopped = true;
        }

        private MessageFlow GetMessageFlowForToken(Token token)
        {
            if (messageFlowsByGuid.ContainsKey(token.Guid)) {
                return messageFlowsByGuid[token.Guid];
            } else {
                MessageFlowConfiguration messageFlowConfiguration = processorService.Configuration.GetMessageFlow(token.Guid);

                MessageFlow messageFlow = new MessageFlow(messageFlowConfiguration, serviceForNode);
                messageFlowsByGuid.Add(messageFlow.Guid, messageFlow);
                return messageFlow;
            }
        }
    }
}
