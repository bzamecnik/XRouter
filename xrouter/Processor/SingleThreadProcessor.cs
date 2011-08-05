using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using XRouter.Common;
using XRouter.Common.MessageFlowConfig;
using XRouter.Processor.MessageFlowParts;

namespace XRouter.Processor
{
    /// <summary>
    /// Implements a processor of tokens in a single thread. It communicates
    /// with other threads via a shared queue where it acts as a consumer.
    /// </summary>
    class SingleThreadProcessor
    {
        /// <summary>
        /// A thread-safe collection of tokens to be processed shared by
        /// producers and consumers of tokens. Some threads can add new
        /// tokens there and it can be observed by threads which perform
        /// the actual processing of tokens.
        /// </summary>
        private BlockingCollection<Token> tokensToProcess;

        /// <summary>
        /// Reference to the main processor component.
        /// </summary>
        internal ProcessorService Processor { get; private set; }

        /// <summary>
        /// A cache of recently used message flows.
        /// </summary>
        private Dictionary<Guid, MessageFlow> messageFlowsByGuid = new Dictionary<Guid, MessageFlow>();

        /// <summary>
        /// Creates a new instance of the single-thread token processor with
        /// given shared collection of tokens.
        /// </summary>
        /// <param name="tokensToProcess">shared collection of tokens to be
        /// processed</param>
        /// <param name="processor">reference to a processor component
        /// </param>
        public SingleThreadProcessor(BlockingCollection<Token> tokensToProcess, ProcessorService processor)
        {
            this.tokensToProcess = tokensToProcess;
            this.Processor = processor;
        }

        /// <summary>
        /// Performs the actual infinite token-processing loop. It waits for
        /// new tokens and processes them.
        /// </summary>
        public void Run()
        {
            foreach (Token token in tokensToProcess.GetConsumingEnumerable()) {
                MessageFlow messageFlow = GetMessageFlowForToken(token);
                bool canContinue = messageFlow.DoStep(token);
                if (canContinue) {
                    tokensToProcess.Add(token);
                } else {
                    Processor.DecrementTokensCount();
                    TraceLog.Info("Processor finished token with GUID " + token.Guid);
                }
            }
        }

        /// <summary>
        /// Obtains a message flow using which given token should be processed.
        /// It creates a message flow from its configuration and uses a cache
        /// for recently used message flows.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private MessageFlow GetMessageFlowForToken(Token token)
        {
            Guid messageFlowGuid = token.MessageFlowState.MessageFlowGuid;

            if (messageFlowsByGuid.ContainsKey(messageFlowGuid)) {
                return messageFlowsByGuid[messageFlowGuid];
            } else {
                MessageFlowConfiguration messageFlowConfiguration = Processor.Configuration.GetMessageFlow(messageFlowGuid);

                MessageFlow messageFlow = new MessageFlow(messageFlowConfiguration, Processor);
                messageFlowsByGuid.Add(messageFlowGuid, messageFlow);
                return messageFlow;
            }
        }
    }
}
