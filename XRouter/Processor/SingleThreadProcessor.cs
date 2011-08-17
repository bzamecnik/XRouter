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
    /// It has a single message flow according which to process the tokens.
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
        /// Message flow - a plan for processing tokens.
        /// </summary>
        private MessageFlow messageFlow;

        /// <summary>
        /// Creates a new instance of the single-thread token processor with
        /// given shared collection of tokens.
        /// </summary>
        /// <param name="tokensToProcess">shared collection of tokens to be
        /// processed</param>
        /// <param name="processor">reference to a processor component
        /// </param>
        public SingleThreadProcessor(
            BlockingCollection<Token> tokensToProcess,
            ProcessorService processor,
            MessageFlow messageFlow)
        {
            this.tokensToProcess = tokensToProcess;
            this.Processor = processor;
            this.messageFlow = messageFlow;

        }

        /// <summary>
        /// Performs the actual infinite token-processing loop. It waits for
        /// new tokens and processes them.
        /// </summary>
        public void Run()
        {
            foreach (Token token in tokensToProcess.GetConsumingEnumerable())
            {
                bool canContinue = messageFlow.DoStep(token);
                if (canContinue)
                {
                    tokensToProcess.Add(token);
                }
                else
                {
                    Processor.DecrementTokensCount();
                    TraceLog.Info("Processor finished token with GUID " + token.Guid);
                }
            }
        }
    }
}
