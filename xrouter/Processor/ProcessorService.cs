using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using XRouter.Broker;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using XRouter.Common.ComponentInterfaces;

namespace XRouter.Processor
{
    public class ProcessorService : IProcessorService
    {
        public string Name { get; private set; }

        internal ApplicationConfiguration Configuration { get; private set; }

        private XmlReduction ConfigReduction { get; set; }

        private IBrokerServiceForProcessor BrokerService { get; set; }

        private ProcessorServiceForNode serviceForNode;

        private BlockingCollection<Token> tokensToProcess;
        private ConcurrentBag<SingleThreadProcessor> concurrentProcessors;

        private object addWorkLock = new object();
        private volatile bool isStopping;
        private int tokensCount;
        private ManualResetEvent tokensFinishedEvent;

        public void Start(string componentName, IBrokerServiceForProcessor brokerService)
        {
            Name = componentName;
            BrokerService = brokerService;

            ConfigReduction = new XmlReduction();

            Configuration = BrokerService.GetConfiguration(ConfigReduction);
            serviceForNode = new ProcessorServiceForNode(Name, BrokerService, Configuration);

            tokensCount = 0;
            tokensFinishedEvent = new ManualResetEvent(true);
            tokensToProcess = new BlockingCollection<Token>(new ConcurrentQueue<Token>());
            isStopping = false;

            #region Create and start concurrentProcessors
            int concurrentThreadsCount = Configuration.GetConcurrentThreadsCountForProcessor(Name);
            concurrentProcessors = new ConcurrentBag<SingleThreadProcessor>();
            for (int i = 0; i < concurrentThreadsCount; i++) {
                Task.Factory.StartNew(delegate {
                    SingleThreadProcessor processor = new SingleThreadProcessor(tokensToProcess, this, serviceForNode);
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

        public void UpdateConfig(ApplicationConfiguration config)
        {
            Configuration = config;
            serviceForNode.Configuration = config;
        }

        public double GetUtilization()
        {
            return 0.5d;
        }

        public void AddWork(Token token)
        {
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

        internal void DecrementTokensCount()
        {
            if (Interlocked.Decrement(ref tokensCount) == 0)
            {
                tokensFinishedEvent.Set();
            }
        }
    }
}
