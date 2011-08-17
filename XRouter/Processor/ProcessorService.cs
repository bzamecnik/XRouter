using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using XRouter.Broker;
using XRouter.Common;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common.MessageFlowConfig;
using XRouter.Processor.MessageFlowParts;

namespace XRouter.Processor
{
    /// <summary>
    /// Multi-threaded implementation of the processor component. It manages
    /// several single-threaded processors which do the actual token
    /// processing.
    /// </summary>
    /// <remarks>
    /// It has a single message flow according which to process the tokens.
    /// </remarks>
    /// <seealso cref="XRouter.Processor.SingleThreadProcessor"/>
    public class ProcessorService : IProcessorService
    {
        /// <summary>
        /// Identifier of the processor component instance.
        /// </summary>
        public string Name { get; private set; }

        internal ApplicationConfiguration Configuration { get; private set; }

        internal IBrokerServiceForProcessor BrokerService { get; private set; }

        private XmlReduction ConfigReduction { get; set; }

        /// <summary>
        /// A thread-safe collection of tokens to be processed shared by
        /// producers and consumers of tokens. This processor component acts
        /// as a producer.
        /// </summary>
        private BlockingCollection<Token> tokensToProcess;

        /// <summary>
        /// References to single-thread processors managed by this processor
        /// component instance.
        /// </summary>
        private ConcurrentBag<SingleThreadProcessor> concurrentProcessors;

        private object addWorkLock = new object();
        private volatile bool isStopping;
        private int tokensCount;
        private ManualResetEvent tokensFinishedEvent;
        private MessageFlow[] messageFlows;

        #region IProcessorService interface

        public void Start(string componentName, IBrokerServiceForProcessor brokerService)
        {
            Name = componentName;
            BrokerService = brokerService;

            ConfigReduction = new XmlReduction();

            Configuration = BrokerService.GetConfiguration(ConfigReduction);

            tokensCount = 0;
            tokensFinishedEvent = new ManualResetEvent(true);
            tokensToProcess = new BlockingCollection<Token>(new ConcurrentQueue<Token>());
            isStopping = false;

            #region Create and start concurrentProcessors
            int concurrentThreadsCount = Configuration.GetConcurrentThreadsCountForProcessor(Name);

            // NOTE: an exception from inicialization should stop the XRouter service
            // NOTE: each SingleThreadProcessor must have its own message flow instance
            // since they might not be thread-safe
            messageFlows = new MessageFlow[concurrentThreadsCount];
            for (int i = 0; i < concurrentThreadsCount; i++)
            {
                messageFlows[i] = GetCurrentMessageFlow();
            }

            concurrentProcessors = new ConcurrentBag<SingleThreadProcessor>();
            for (int i = 0; i < concurrentThreadsCount; i++) {
                // NOTE: it is essential to copy the index variable
                // to prevent using a closure
                int processorIndex = i;
                // NOTE: exceptions there do not stop the XRouter service
                Task.Factory.StartNew(TraceLog.WrapWithExceptionLogging(
                    delegate{
                        TraceLog.Info("processor " + processorIndex);
                        SingleThreadProcessor processor = new SingleThreadProcessor(
                            tokensToProcess, this, messageFlows[processorIndex]);
                        concurrentProcessors.Add(processor);
                        processor.Run();
                    }),
                    TaskCreationOptions.LongRunning);
            }
            #endregion
        }

        public void Stop()
        {
            lock (addWorkLock)
            {
                isStopping = true;
                tokensFinishedEvent.WaitOne();
                tokensToProcess.CompleteAdding();
            }
        }

        public double GetUtilization()
        {
            return 0.5d;
        }

        public void AddWork(Token token)
        {
            TraceLog.Info("Processor received a token with GUID " + token.Guid);
            lock (addWorkLock)
            {
                if (isStopping)
                {
                    throw new InvalidOperationException(string.Format(
                        "Cannot add token with GUID '{0}' the a processor because it is just stopping.",
                        token.Guid));
                }
                if (token.State != TokenState.Finished)
                {
                    tokensFinishedEvent.Reset();
                    Interlocked.Increment(ref tokensCount);
                    tokensToProcess.Add(token);
                }
            }
        }

        #endregion

        #region IComponentService interface

        public void UpdateConfig(ApplicationConfiguration config)
        {
            Configuration = config;
        }

        #endregion

        internal void DecrementTokensCount()
        {
            if (Interlocked.Decrement(ref tokensCount) == 0)
            {
                tokensFinishedEvent.Set();
            }
        }

        private MessageFlow GetCurrentMessageFlow()
        {
            Guid messageFlowId = Configuration.GetCurrentMessageFlowGuid();
            var config = Configuration.GetMessageFlow(messageFlowId);
            return new MessageFlow(config, this);
        }
    }
}
