using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleDiagrammer;
using XRouter.Common.MessageFlowConfig;

namespace XRouter.Gui.ConfigurationControls.Messageflow
{
    class MessageflowGraphPresenter : GraphPresenter<NodeConfiguration, Tuple<NodeConfiguration, NodeConfiguration>>
    {
        private MessageFlowConfiguration messageflow;

        public MessageflowGraphPresenter(MessageFlowConfiguration messageflow)
        {
            this.messageflow = messageflow;
        }

        public override IEnumerable<NodeConfiguration> GetNodes()
        {
            return messageflow.Nodes;
        }

        public override IEnumerable<Tuple<NodeConfiguration, NodeConfiguration>> GetEdges()
        {
            var result = new List<Tuple<NodeConfiguration,NodeConfiguration>>();
            foreach (var node in messageflow.Nodes) {
                if (node is CbrNodeConfiguration) {
                    var cbrNode = (CbrNodeConfiguration)node;
                    foreach (var targetNode in cbrNode.Branches.Values) {
                        result.Add(Tuple.Create(node, targetNode));
                    }
                    result.Add(Tuple.Create(node, cbrNode.DefaultTarget));
                } else if (node is ActionNodeConfiguration) {
                    var actionNode = (ActionNodeConfiguration)node;
                    result.Add(Tuple.Create(node, actionNode.NextNode));
                }
            }
            return result.Where(t => !(t.Item2 is TerminatorNodeConfiguration));
        }

        public override NodePresenter<NodeConfiguration> CreateNodePresenter(NodeConfiguration node)
        {
            return new MessageflowNodePresenter(node);
        }

        public override EdgePresenter<NodeConfiguration, Tuple<NodeConfiguration, NodeConfiguration>> CreateEdgePresenter(Tuple<NodeConfiguration, NodeConfiguration> edge)
        {
            return new MessageflowEdgePresenter(edge, edge.Item1, edge.Item2);
        }
    }
}
