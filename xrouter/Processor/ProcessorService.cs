using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using XRouter.Broker;
using XRouter.Common;
using XRouter.Common.ComponentInterfaces;

namespace XRouter.Processor
{
    /// <summary>
    /// Multi-threaded implementation of the processor component. It manages
    /// several single-threaded processors which do the actual token
    /// processing.
    /// </summary>
    /// <seealso cref="XRouter.Processor.SingleThreadProcessor"/>
    public class ProcessorService : IProcessorService
    {
        public string Name { get; private set; }

        internal ApplicationConfiguration Configuration { get; private set; }

        internal IBrokerServiceForProcessor BrokerService { get; private set; }

        private XmlReduction ConfigReduction { get; set; }

        private BlockingCollection<Token> tokensToProcess;
        private ConcurrentBag<SingleThreadProcessor> concurrentProcessors;

        private object addWorkLock = new object();
        private volatile bool isStopping;
        private int tokensCount;
        private ManualResetEvent tokensFinishedEvent;

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
            concurrentProcessors = new ConcurrentBag<SingleThreadProcessor>();
            for (int i = 0; i < concurrentThreadsCount; i++) {
                Task.Factory.StartNew(delegate {
                    SingleThreadProcessor processor = new SingleThreadProcessor(tokensToProcess, this);
                    concurrentProcessors.Add(processor);
                    processor.Run();
                }, TaskCreationOptions.LongRunning);
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
                    throw new InvalidOperationException("Cannot add token because processor is stopping.");
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
    }
}
