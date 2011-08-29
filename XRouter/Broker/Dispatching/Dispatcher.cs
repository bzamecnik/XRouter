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

        private IBrokerServiceForDispatcher brokerService;
        private ProcessorAccessor processor;

        private TaskFactory tokenDispatchingTaskFactory;
        private object tokenDispatchingLock = new object();

        private volatile bool isStopping;

        private Guid messageFlowGuid;

        internal Dispatcher(IBrokerServiceForDispatcher brokerService)
        {
            this.brokerService = brokerService;
            messageFlowGuid = brokerService.GetConfiguration().GetCurrentMessageFlowGuid();

            processor = brokerService.GetProcessors().First();

            tokenDispatchingTaskFactory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(
                MaxTokenDispatchingConcurrencyLevel));
        }

        /// <summary>
        /// Dispatches the token to the processor.
        /// </summary>
        /// <param name="token"></param>
        public void Dispatch(Token token)
        {
            if (isStopping)
            {
                return;
            }
            lock (tokenDispatchingLock)
            {
                MessageFlowState messageflowState = token.GetMessageFlowState();
                if (messageflowState.AssignedProcessor != null)
                {
                    return; // Token is already dispatched
                }

                // NOTE: it compares to default GUID returned by the new Guid()
                if (messageflowState.MessageFlowGuid == new Guid())
                {
                    token = brokerService.UpdateTokenMessageFlow(token.Guid, messageFlowGuid);
                }
                try
                {
                    TraceLog.Info(string.Format(
                        "Dispatcher assigning token '{0}' to processor '{1}'",
                        token.Guid, processor.ComponentName));
                    token = brokerService.UpdateTokenLastResponseFromProcessor(token.Guid, DateTime.Now);
                    token = brokerService.UpdateTokenAssignedProcessor(token.Guid, processor.ComponentName);
                    token.State = TokenState.InProcessor;
                    processor.AddWork(token);
                }
                catch
                {
                    // NOTE: token retains the Received state
                    brokerService.UpdateTokenAssignedProcessor(token.Guid, null);
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
        }
    }
}
