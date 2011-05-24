using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using XRouter.Broker;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace XRouter.Processor
{
    class ProcessorService : IProcessorService, IHostableComponent
    {
        public string Name { get; private set; }

        public ApplicationConfiguration Configuration { get; private set; }

        public event Action<string> LogEventInfo;
        public event Action<string> LogEventWarning;
        public event Action<string> LogEventError;
        public event Action<string> LogTraceInfo;
        public event Action<string> LogTraceWarning;
        public event Action<string> LogTraceError;
        public event Action<Exception> LogTraceException;

        private XmlReduction ConfigReduction { get; set; }

        private IBrokerServiceForProcessor BrokerService { get; set; }

        private ProcessorServiceForNode serviceForNode;

        private BlockingCollection<Token> tokensToProcess;
        private ConcurrentBag<SingleThreadProcessor> concurrentProcessors;

        public void Start(string componentName, IDictionary<string, string> settings)
        {
            Name = componentName;
            BrokerService = null;

            ConfigReduction = new XmlReduction();

            Configuration = BrokerService.GetConfiguration(ConfigReduction);
            serviceForNode = new ProcessorServiceForNode(Name, BrokerService, Configuration);

            tokensToProcess = new BlockingCollection<Token>(new ConcurrentQueue<Token>());

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
            tokensToProcess.CompleteAdding();
            foreach (var processor in concurrentProcessors) {
                processor.Stop();
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
            if (token.State != TokenState.Finished) {
                tokensToProcess.Add(token);
            }
        }
    }
}
