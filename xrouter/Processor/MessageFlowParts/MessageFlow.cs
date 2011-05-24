using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common.MessageFlow;
using XRouter.Common;

namespace XRouter.Processor.MessageFlowParts
{
    class MessageFlow
    {
        public Guid Guid { get { return Configuration.Guid; } }

        private MessageFlowConfiguration Configuration { get; set; }

        private ProcessorServiceForNode ProcessorService { get; set; }

        private Node rootNode;

        private Dictionary<string, Node> nodesByName = new Dictionary<string, Node>();        

        public MessageFlow(MessageFlowConfiguration configuration, ProcessorServiceForNode processorService)
        {
            Configuration = configuration;
            ProcessorService = processorService;

            #region Create nodes
            nodesByName = new Dictionary<string, Node>();
            foreach (NodeConfiguration nodeCfg in configuration.Nodes) {
                Node node;
                if (nodeCfg is CbrNodeConfiguration) {
                    node = new CbrNode();
                }else if (nodeCfg is ActionNodeConfiguration) {
                    node = new ActionNode();
                } else if (nodeCfg is TerminatorNodeConfiguration) {
                    node = new TerminatorNode();
                } else {
                    throw new InvalidOperationException("Unknown node type");
                }

                node.Initialize(nodeCfg, ProcessorService);
                nodesByName.Add(node.Name, node);
            }
            #endregion

            rootNode = nodesByName[Configuration.RootNode.Name];
        }

        public bool DoStep(Token token)
        {
            #region Determine currentNode
            Node currentNode;
            if (token.MessageFlowState.NextNodeName != null) {
                currentNode = nodesByName[token.MessageFlowState.NextNodeName];
            } else {
                currentNode = rootNode;
            }
            #endregion

            string nextNodeName = currentNode.Evaluate(token);
            token.MessageFlowState.NextNodeName = nextNodeName;
            token.SaveMessageFlowState();

            if (nextNodeName == null) {
                return false;
            } else {
                return true;
            }
        }
    }
}
