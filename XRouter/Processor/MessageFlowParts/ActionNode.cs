using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XRouter.Common;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Utils;

namespace XRouter.Processor.MessageFlowParts
{
    /// <summary>
    /// Represents a node in the message flow which performs one or more
    /// actions with the processed token in parallel.
    /// </summary>
    /// <remarks>
    /// Actions to be performed in this node are implemented as action
    /// plugins and configured specifically for each action node.
    /// </remarks>
    /// <seealso cref="XRouter.Processor.IActionPlugin"/>
    /// <seealso cref="XRouter.Common.MessageFlowConfig.ActionNodeConfiguration"/>
    class ActionNode : Node
    {
        private ActionNodeConfiguration Config { get; set; }

        private List<IActionPlugin> actions = new List<IActionPlugin>();

        /// <summary>
        /// Initializes an action node.
        /// </summary>
        /// <param name="configuration">ActionNodeConfiguration is expected
        /// </param>
        public override void InitializeCore(NodeConfiguration configuration)
        {
            Config = (ActionNodeConfiguration)configuration;

            foreach (ActionConfiguration actionConfig in Config.Actions)
            {
                ActionType actionType = ProcessorService.Configuration.GetActionType(actionConfig.ActionTypeName);
                IActionPlugin action = TypeUtils.CreateTypeInstance<IActionPlugin>(actionType.ClrTypeAndAssembly);

                ObjectConfigurator.Configurator.LoadConfiguration(action, actionConfig.Configuration);

                action.Initialize(ProcessorService);
                actions.Add(action);
            }
        }

        public override string Evaluate(ref Token token)
        {
            TraceLog.Info("Evaluating action: " + Name);
            // NOTE: ref parameter of the method can't be directly passed into
            // a lambda function
            //Token workingToken = token;
            //Parallel.ForEach(actions, delegate(IActionPlugin action)
            //{
            //    try
            //    {
            //        action.Evaluate(ref workingToken);
            //    }
            //    catch (Exception ex)
            //    {
            //        ProcessorService.AddExceptionToToken(workingToken.Guid, ex, out workingToken);
            //    }
            //});

            // TODO: token reference must not be modified when using it in parallel
            // Until the problem is solved use just sequential for each loop.

            foreach (var action in actions)
            {
                try
                {
                    action.Evaluate(ref token);
                }
                catch (Exception ex)
                {
                    ProcessorService.AddExceptionToToken(token.Guid, ex, out token);
                }
            }

            return Config.NextNode.Name;
        }
    }
}
