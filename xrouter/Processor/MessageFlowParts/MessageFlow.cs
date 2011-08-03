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
                    throw new InvalidOperationException("Unknown node type");
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
        public bool DoStep(Token token)
        {
            #region Determine currentNode
            Node currentNode;
            if (token.MessageFlowState.NextNodeName != null)
            {
                currentNode = nodesByName[token.MessageFlowState.NextNodeName];
            }
            else
            {
                currentNode = rootNode;
            }
            #endregion

            string nextNodeName = currentNode.Evaluate(token);

            // TODO ulozit pokud je persistentni akce

            token.MessageFlowState.NextNodeName = nextNodeName;
            token.SaveMessageFlowState();

            bool shouldContinue = nextNodeName != null;
            return shouldContinue;
        }
    }
}
