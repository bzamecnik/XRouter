using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Prototype.CoreTypes;
using XRouter.Prototype.Processor;
using System.Threading.Tasks;
using System.Threading;

namespace XRouter.Prototype.CoreServices
{
    class Dispatcher
    {
        private bool IsRunnig { get; set; }

        private Func<ApplicationConfiguration> GetConfiguration { get; set; }
        private Func<string, Token[]> GetTokensAssignedToProcessor { get; set; }

        private object stateLock = new object();
        private List<ComponentInfo> processorsInfo = new List<ComponentInfo>();
        private Task unresponsibleProcessorsChecking;

        public Dispatcher(Func<ApplicationConfiguration> getConfiguration, Func<string, Token[]> getTokensAssignedToProcessor)
        {
            GetConfiguration = getConfiguration;
            GetTokensAssignedToProcessor = getTokensAssignedToProcessor;

            unresponsibleProcessorsChecking = Task.Factory.StartNew(CheckUnresponsibleProcessors, TaskCreationOptions.LongRunning);
        }

        public void Start()
        {
            IsRunnig = true;
        }

        public void Stop()
        {
            IsRunnig = false;
        }

        private void CheckUnresponsibleProcessors()
        {
            while (true) {
                Thread.Sleep(TimeSpan.FromSeconds(60));
                if (IsRunnig) {
                    ComponentInfo[] availableProcessorsInfo;
                    lock (stateLock) {
                        availableProcessorsInfo = processorsInfo.ToArray();
                    }

                    var config = GetConfiguration();
                    foreach (var processorInfo in availableProcessorsInfo) {
                        processorInfo.UpdateState();
                        if (processorInfo.State.Status != ComponentStatus.Running) {
                            var tokens = GetTokensAssignedToProcessor(processorInfo.Name);
                            DateTime lastResponseThreshold = DateTime.Now - config.GetNonRunningProcessorResponseTimeout();
                            var tokensToRedispatch = tokens.Where(t => t.WorkflowState.LastResponseFromProcessor < lastResponseThreshold);
                            foreach (var tokenToRedispatch in tokensToRedispatch) {
                                Dispatch(tokenToRedispatch);
                            }
                        }
                    }
                }
            }
        }

        public void UpdateProcessorsInfo(IEnumerable<ComponentInfo> processorsInfo)
        {
            lock (stateLock) {
                this.processorsInfo.Clear();
                this.processorsInfo.AddRange(processorsInfo);
            }
        }

        public void Dispatch(Token token)
        {
            var config = GetConfiguration();
            ComponentInfo[] availableProcessorsInfo;
            lock (stateLock) {
                availableProcessorsInfo = processorsInfo.Where(p => p.State.Status == ComponentStatus.Running).ToArray();
            }

            IProcessor chosenProcessor = null;
            ComponentInfo choosenProcessorInfo = null;
            double minUtilization = double.MaxValue;
            foreach (var processorInfo in availableProcessorsInfo) {
                IProcessor processor = processorInfo.GetProcessorProxy();
                double utilization = processor.GetUtilization();
                if (utilization < minUtilization) {
                    minUtilization = utilization;
                    chosenProcessor = processor;
                    choosenProcessorInfo = processorInfo;
                }
            }
            if (choosenProcessorInfo == null) {
                return;
            }

            if (token.WorkflowState.WorkflowVersion == 0) {
                token.WorkflowState.WorkflowVersion = config.GetLastWorkflowVersion();
            }
            token.WorkflowState.AssignedProcessor = choosenProcessorInfo.Name; // It is important to change AssignedProcessor property before sending token to processor
            chosenProcessor.AddWork(token);
        }
    }
}
