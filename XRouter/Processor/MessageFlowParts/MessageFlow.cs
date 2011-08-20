using System;
using System.Collections.Generic;
using XRouter.Common;
using XRouter.Common.MessageFlowConfig;

namespace XRouter.Processor.MessageFlowParts
{
    /// <summary>
    /// Represents a message flow which is an rooted oriented graph of nodes
    /// directing the course of processing a token.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each message flow is uniquely identified by a GUID.
    /// There is one active message flow in the processor which is used for
    /// new tokens. If the active message flow is changed tokens retain the
    /// original message flow with which they were started.
    /// The graph can contain cycles.
    /// </para>
    /// <para>
    /// A message flow instance may not be thread-safe since it can contain
    /// arbitratry actions, even non thread-safe ones. So that each processor
    /// thread needs its own message flow instance.
    /// </para>
    /// </remarks>
    class MessageFlow
    {
        /// <summary>
        /// Uniquely identifies the message flow with a GUID.
        /// </summary>
        public Guid Guid { get { return Configuration.Guid; } }

        private MessageFlowConfiguration Configuration { get; set; }

        private ProcessorService Processor { get; set; }

        private Node rootNode;

        private Dictionary<string, Node> nodesByName = new Dictionary<string, Node>();

        public MessageFlow(MessageFlowConfiguration configuration, ProcessorService processor)
        {
            Configuration = configuration;
            Processor = processor;

            #region Create nodes
            nodesByName = new Dictionary<string, Node>();
            foreach (NodeConfiguration nodeCfg in configuration.Nodes)
            {
                Node node;
                if (nodeCfg is CbrNodeConfiguration)
                {
                    node = new CbrNode();
                }
                else if (nodeCfg is ActionNodeConfiguration)
                {
                    node = new ActionNode();
                }
                else if (nodeCfg is TerminatorNodeConfiguration)
                {
                    node = new TerminatorNode();
                }
                else
                {
                    throw new InvalidOperationException(string.Format(
                        "Cannot create a node named '{0}' of unknown node type.", nodeCfg.Name));
                }

                node.Initialize(nodeCfg, Processor);
                nodesByName.Add(node.Name, node);
            }
            #endregion

            rootNode = nodesByName[Configuration.RootNode.Name];
        }

        /// <summary>
        /// Performs a single step in the message flow graph traversal. This
        /// corresponds to moving along an edge from one node to another.
        /// </summary>
        /// <remarks>
        /// After each step the message flow state is saved to the token.
        /// </remarks>
        /// <param name="token">token to be processed</param>
        /// <returns>true if the traversal should continue; false if it has
        /// finished</returns>
        public bool DoStep(ref Token token)
        {
            #region Determine currentNode
            Node currentNode;
            string currentNodeName = token.GetMessageFlowState().NextNodeName;
            if (currentNodeName != null) {
                currentNode = nodesByName[currentNodeName];
            }
            else {
                currentNode = rootNode;
            }
            #endregion

            string nextNodeName = currentNode.Evaluate(ref token);

            MessageFlowState messageflowState = token.GetMessageFlowState();
            messageflowState.NextNodeName = nextNodeName;
            token.SetMessageFlowState(messageflowState);

            if ((token.IsPersistent) && (!(currentNode is CbrNode))) {
                Processor.BrokerService.UpdateTokenMessageFlowState(
                    Processor.Name, token.Guid, messageflowState, out token);
            }

            bool shouldContinue = nextNodeName != null;
            return shouldContinue;
        }
    }
}
