using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;
using System.Threading.Tasks;
using System.Threading;

namespace XRouter.Broker.Dispatching
{
    internal class Dispatcher
    {
        private static readonly int MaxTokenDispatchingConcurrencyLevel = 10;
        private static readonly TimeSpan BackgroundCheckingInterval = TimeSpan.FromSeconds(60);

        private IBrokerServiceForDispatcher brokerService;

        private TaskFactory tokenDispatchingTaskFactory;
        private object tokenDispatchingLock = new object();

        private Task backgroundCheckings;
        private bool isStopping;

        internal Dispatcher(IBrokerServiceForDispatcher brokerService)
        {
            this.brokerService = brokerService;

            tokenDispatchingTaskFactory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(MaxTokenDispatchingConcurrencyLevel));
            backgroundCheckings = Task.Factory.StartNew(StartBackgroundCheckings, TaskCreationOptions.LongRunning);
        }

        public void NotifyAboutNewToken(Token token)
        {
            DispatchAsync(token.Guid);
        }

        private void DispatchAsync(Guid tokenGuid, Func<ProcessorAccessor, bool> filter = null)
        {
            tokenDispatchingTaskFactory.StartNew(delegate {
                Dispatch(tokenGuid, filter);
            });
        }

        private void Dispatch(Guid tokenGuid, Func<ProcessorAccessor, bool> filter = null)
        {
            #region Determine processorsByUtilization
            var processorsByUtilization = brokerService.GetProcessors().OrderBy(delegate(ProcessorAccessor p) {
                double utilization = double.MaxValue;
                try {
                    utilization = p.GetUtilization();
                } catch { // Keep max utilization for inaccessible processor
                }
                return utilization;
            });
            #endregion

            var config = brokerService.GetConfiguration();
            foreach (var processor in processorsByUtilization) {
                if ((filter == null) || (filter(processor))) {
                    lock (tokenDispatchingLock) {
                        Token token = brokerService.GetToken(tokenGuid);
                        if (token.MessageFlowState.AssignedProcessor != null) {
                            return; // Token is already dispatched
                        }

                        if (token.MessageFlowState.MessageFlowGuid == new Guid()) {
                            Guid messageFlowGuid = config.GetCurrentMessageFlowGuid();
                            brokerService.UpdateTokenMessageFlow(tokenGuid, messageFlowGuid);
                            token.MessageFlowState.MessageFlowGuid = messageFlowGuid;
                        }
                        try {
                            TraceLog.Info(string.Format("Dispatcher assigning token '{0}' to processor '{1}'", token.Guid, processor.ComponentName));
                            processor.AddWork(token);
                        } catch {
                            continue; // if adding token to processor fails, try next one
                        }
                        brokerService.UpdateTokenLastResponseFromProcessor(tokenGuid, DateTime.Now);
                        brokerService.UpdateTokenAssignedProcessor(tokenGuid, processor.ComponentName);
                        return;
                    }
                }
            }
        }

        private void StartBackgroundCheckings()
        {
            while (!isStopping) {
                Thread.Sleep(BackgroundCheckingInterval);

                #region Check unresponsible processors
                try {
                    var config = brokerService.GetConfiguration();
                    foreach (var processor in brokerService.GetProcessors()) {
                        var tokens = brokerService.GetActiveTokensAssignedToProcessor(processor.ComponentName);
                        DateTime lastResponseThreshold = DateTime.Now - config.GetNonRunningProcessorResponseTimeout();
                        var tokensToRedispatch = tokens.Where(t => t.MessageFlowState.LastResponseFromProcessor < lastResponseThreshold);
                        Func<ProcessorAccessor, bool> currentProcessorFilter = p => p.ComponentName != processor.ComponentName;

                        foreach (var tokenToRedispatch in tokensToRedispatch) {
                            tokenToRedispatch.MessageFlowState.AssignedProcessor = null;
                            brokerService.UpdateTokenAssignedProcessor(tokenToRedispatch.Guid, null);
                            DispatchAsync(tokenToRedispatch.Guid, currentProcessorFilter);
                        }
                    }
                } catch (Exception ex) { // Prevent termination of checking thread
                }
                #endregion

                #region Dispatch undispatched tokens
                try {
                    foreach (Token token in brokerService.GetUndispatchedTokens()) {
                        DispatchAsync(token.Guid);
                    }
                } catch (Exception ex) { // Prevent termination of checking thread
                }
                #endregion
            }
        }

        public void Stop()
        {
            isStopping = true;
        }
    }
}
