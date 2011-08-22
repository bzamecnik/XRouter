using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleDiagrammer;
using XRouter.Common.MessageFlowConfig;
using System.Windows;

namespace XRouter.Gui.ConfigurationControls.Messageflow
{
    class MessageflowGraphPresenter : GraphPresenter<NodeConfiguration, Tuple<NodeConfiguration, NodeConfiguration>>
    {
        internal MessageFlowConfiguration Messageflow { get; private set; }

        internal NodeSelectionManager NodeSelectionManager { get; private set; }

        private Dictionary<NodeConfiguration, WeakReference> nodeToPresenter = new Dictionary<NodeConfiguration, WeakReference>();

        public MessageflowGraphPresenter(MessageFlowConfiguration messageflow, NodeSelectionManager nodeSelectionManager)
        {
            Messageflow = messageflow;
            NodeSelectionManager = nodeSelectionManager;
        }

        public MessageflowNodePresenter GetNodePresenter(NodeConfiguration node)
        {
            if (nodeToPresenter.ContainsKey(node)) {
                return (MessageflowNodePresenter)nodeToPresenter[node].Target;
            } else {
                return null;
            }
        }

        public IEnumerable<MessageflowNodePresenter> GetNodePresenters()
        {
            var result = nodeToPresenter.Values.Select(wr => wr.Target as MessageflowNodePresenter).Where(p => p != null).ToArray();
            return result;
        }

        public void HighlightNodes(params NodeConfiguration[] nodes)
        {
            if (nodes.Length == 0) {
                foreach (var nodePresenter in GetNodePresenters()) {
                    ((FrameworkElement)nodePresenter.Content).Opacity = 1;
                }
            } else {
                foreach (var nodePresenter in GetNodePresenters()) {
                    ((FrameworkElement)nodePresenter.Content).Opacity = 0.3;
                }
                foreach (var nodePresenter in nodes.Where(n => n != null).Select(n => GetNodePresenter(n))) {
                    if (nodePresenter != null) {
                        ((FrameworkElement)nodePresenter.Content).Opacity = 1;
                    }
                }
            }
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
                } else if (node is EntryNodeConfiguration) {
                    var entryNode = (EntryNodeConfiguration)node;
                    result.Add(Tuple.Create(node, entryNode.NextNode));
                }
            }

            return result
                .Where(e => e.Item2 != null);
        }

        public override NodePresenter<NodeConfiguration> CreateNodePresenter(NodeConfiguration node)
        {
            var result = new MessageflowNodePresenter(node, this, NodeSelectionManager);
            
            #region Update nodeToPresenter
            var deadNodes = nodeToPresenter.Where(p => !p.Value.IsAlive).Select(p => p.Key).ToArray();
            foreach (var deadNode in deadNodes) {
                nodeToPresenter.Remove(deadNode);
            }
            nodeToPresenter.Add(node, new WeakReference(result));
            #endregion

            return result;
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
