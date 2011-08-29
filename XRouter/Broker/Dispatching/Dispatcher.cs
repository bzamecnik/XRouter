using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XRouter.Common;

namespace XRouter.Broker.Dispatching
{
    /// <summary>
    /// Dispatcher is a part of the broker responsible for assigning tokens
    /// to processors. Also it can reassign tokens from dead or too busy
    /// processors and lost tokens which are not assigned for any reason.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Dispatcher can be used via the NotifyAboutNewToken() method. When no
    /// longer needed it can be stopped via the Stop() method.</para>
    /// <para>The periodical check for unresponsible processors and unassigned
    /// tokens happens in a background thread.</para>
    /// </remarks>
    internal class Dispatcher
    {
        private static readonly int MaxTokenDispatchingConcurrencyLevel = 10;
        private static readonly TimeSpan BackgroundCheckingInterval = TimeSpan.FromSeconds(60);

        private IBrokerServiceForDispatcher brokerService;

        private TaskFactory tokenDispatchingTaskFactory;
        private object tokenDispatchingLock = new object();

        private Task backgroundCheckings;
        private volatile bool isStopping;

        Guid messageFlowGuid;

        internal Dispatcher(IBrokerServiceForDispatcher brokerService)
        {
            this.brokerService = brokerService;
            messageFlowGuid = brokerService.GetConfiguration().GetCurrentMessageFlowGuid();

            tokenDispatchingTaskFactory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(
                MaxTokenDispatchingConcurrencyLevel));

            // NOTE: Checking for unresponsible processors and lost tokens
            // has been disabled by a specification update.

            // NOTE: the thread must not die on and exceptions are handled inside
            // so it's not wrapped using TraceLog.WrapWithExceptionLogging()
            //backgroundCheckings = Task.Factory.StartNew(StartBackgroundCheckings,
            //    TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Notifies the dispatcher that it should dispatch given token.
        /// The actual dispatching happens asynchronously, so this method does
        /// not wait until the token is dispatched.
        /// </summary>
        /// <param name="token"></param>
        public void NotifyAboutNewToken(Token token)
        {
            //DispatchAsync(token.Guid);
            Dispatch(token.Guid);
        }

        private void DispatchAsync(Guid tokenGuid, Func<ProcessorAccessor, bool> filter = null)
        {
            tokenDispatchingTaskFactory.StartNew(
                TraceLog.WrapWithExceptionLogging(
                    delegate { Dispatch(tokenGuid, filter); })
            );
        }

        private void Dispatch(Guid tokenGuid, Func<ProcessorAccessor, bool> filter = null)
        {
            #region Determine processorsByUtilization
            var processorsByUtilization = brokerService.GetProcessors().OrderBy(delegate(ProcessorAccessor p)
            {
                double utilization = double.MaxValue;
                try
                {
                    utilization = p.GetUtilization();
                }
                catch
                { // Keep max utilization for inaccessible processors
                }
                return utilization;
            });
            #endregion

            foreach (var processor in processorsByUtilization)
            {
                if ((filter == null) || (filter(processor)))
                {
                    lock (tokenDispatchingLock)
                    {
                        Token token = brokerService.GetToken(tokenGuid);
                        MessageFlowState messageflowState = token.GetMessageFlowState();
                        if (messageflowState.AssignedProcessor != null)
                        {
                            return; // Token is already dispatched
                        }

                        // NOTE: it compares to default GUID returned by the new Guid()
                        if (messageflowState.MessageFlowGuid == new Guid())
                        {
                            token = brokerService.UpdateTokenMessageFlow(tokenGuid, messageFlowGuid);
                        }
                        try
                        {
                            TraceLog.Info(string.Format(
                                "Dispatcher assigning token '{0}' to processor '{1}'",
                                token.Guid, processor.ComponentName));
                            token = brokerService.UpdateTokenLastResponseFromProcessor(tokenGuid, DateTime.Now);
                            token = brokerService.UpdateTokenAssignedProcessor(tokenGuid, processor.ComponentName);
                            token.State = TokenState.InProcessor;
                            processor.AddWork(token);
                        }
                        catch
                        {
                            // NOTE: token retains the Received state
                            brokerService.UpdateTokenAssignedProcessor(tokenGuid, null);
                            continue; // if adding token to processor fails, try next one
                        }
                        return; // token has been successfully dispatched
                    }
                }
            }
        }

        /// <summary>
        /// Periodially check for processors which did not respond more than
        /// some theshold time (unresponsible ones) and reassign their tokens
        /// to other processors. Moreover, dispatch any unassigned tokens
        /// again.
        /// </summary>
        private void StartBackgroundCheckings()
        {
            while (!isStopping)
            {
                Thread.Sleep(BackgroundCheckingInterval);

                try
                {
                    #region Check unresponsible processors
                    var config = brokerService.GetConfiguration();
                    // reassign:
                    // - unassign tokens from unresponsible processor
                    // - dispatch unassigned tokens again
                    foreach (var processor in brokerService.GetProcessors())
                    {
                        try
                        {
                            var tokens = brokerService.GetActiveTokensAssignedToProcessor(processor.ComponentName);
                            DateTime lastResponseThreshold = DateTime.Now -
                                config.GetNonRunningProcessorResponseTimeout();
                            var tokensToRedispatch = tokens.Where(
                                t => t.GetMessageFlowState().LastResponseFromProcessor < lastResponseThreshold);
                            Func<ProcessorAccessor, bool> currentProcessorFilter =
                                p => p.ComponentName != processor.ComponentName;

                            foreach (var tokenToRedispatch in tokensToRedispatch)
                            {
                                tokenToRedispatch.UpdateMessageFlowState(mfs => { mfs.AssignedProcessor = null; });
                                brokerService.UpdateTokenAssignedProcessor(tokenToRedispatch.Guid, null);
                                DispatchAsync(tokenToRedispatch.Guid, currentProcessorFilter);
                            }
                        }
                        catch (Exception ex)
                        {
                            // prevent termination of checking thread, just log the error
                            TraceLog.Exception(ex);
                        }
                    }
                    #endregion

                    #region Dispatch undispatched tokens

                    foreach (Token token in brokerService.GetUndispatchedTokens())
                    {
                        try
                        {
                            DispatchAsync(token.Guid);
                        }
                        catch (Exception ex)
                        {
                            // prevent termination of checking thread, just log the error
                            TraceLog.Exception(ex);
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    // prevent termination of checking thread, just log the error
                    TraceLog.Exception(ex);
                }
            }
        }

        /// <summary>
        /// Stops the dispatcher. Once it is stopped it cannot be started
        /// again.
        /// </summary>
        public void Stop()
        {
            isStopping = true;
            // NOTE: this finishes the StartBackgroundCheckings() thread
        }
    }
}
