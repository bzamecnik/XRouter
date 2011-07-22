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
        internal MessageFlowConfiguration Messageflow { get; private set; }

        internal NodeSelectionManager NodeSelectionManager { get; private set; }

        public MessageflowGraphPresenter(MessageFlowConfiguration messageflow, NodeSelectionManager nodeSelectionManager)
        {
            Messageflow = messageflow;
            NodeSelectionManager = nodeSelectionManager;
        }

        public override IEnumerable<NodeConfiguration> GetNodes()
        {
            return Messageflow.Nodes;
        }

        public override IEnumerable<Tuple<NodeConfiguration, NodeConfiguration>> GetEdges()
        {
            var result = new List<Tuple<NodeConfiguration, NodeConfiguration>>();
            foreach (var node in Messageflow.Nodes) {
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

            return result
                .Where(e => e.Item2 != null);
        }

        public override NodePresenter<NodeConfiguration> CreateNodePresenter(NodeConfiguration node)
        {
            return new MessageflowNodePresenter(node, this, NodeSelectionManager);
        }

        public override EdgePresenter<NodeConfiguration, Tuple<NodeConfiguration, NodeConfiguration>> CreateEdgePresenter(Tuple<NodeConfiguration, NodeConfiguration> edge)
        {
            return new MessageflowEdgePresenter(edge, edge.Item1, edge.Item2);
        }

        public new void RaiseGraphChanged()
        {
            base.RaiseGraphChanged();
        }
    }
}
