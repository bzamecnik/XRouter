using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using XRouter.Common.MessageFlowConfig;

namespace XRouter.Gui.ConfigurationControls.Messageflow
{
    delegate void NodeSelectingHandler(object sender, NodeSelectingEventArgs e);
    class NodeSelectingEventArgs : EventArgs
    {
        public NodeConfiguration OriginalSelectedNode { get; private set; }
        public NodeConfiguration NewSelectedNode { get; private set; }
        public bool IsCancelled { get; set; }

        public NodeSelectingEventArgs(NodeConfiguration originalSelectedNode, NodeConfiguration newSelectedNode)
        {
            OriginalSelectedNode = originalSelectedNode;
            NewSelectedNode = newSelectedNode;
        }
    }

    delegate void NodeSelectedHandler(object sender, NodeSelectedEventArgs e);
    class NodeSelectedEventArgs : EventArgs
    {
        public NodeConfiguration OriginalSelectedNode { get; private set; }
        public NodeConfiguration NewSelectedNode { get; private set; }

        public NodeSelectedEventArgs(NodeConfiguration originalSelectedNode, NodeConfiguration newSelectedNode)
        {
            OriginalSelectedNode = originalSelectedNode;
            NewSelectedNode = newSelectedNode;
        }
    }

    class NodeSelectionManager
    {
        public event NodeSelectingHandler NodeSelecting = delegate { };
        public event NodeSelectedHandler NodeSelected = delegate { };

        public NodeConfiguration SelectedNode { get; private set; }

        internal MessageflowGraphPresenter MessageflowGraphPresenter { get; set; }

        private ContentControl propertiesContainer;

        public NodeSelectionManager(ContentControl propertiesContainer)
        {
            this.propertiesContainer = propertiesContainer;
        }

        public void SelectNode(NodeConfiguration selectedNode)
        {
            NodeConfiguration originalSelectedNode = SelectedNode;

            NodeSelectingEventArgs nodeSelectingEventArgs = new NodeSelectingEventArgs(originalSelectedNode, selectedNode);
            NodeSelecting(this, nodeSelectingEventArgs);
            if (nodeSelectingEventArgs.IsCancelled) {
                return;
            }

            if (selectedNode is ActionNodeConfiguration) {
                propertiesContainer.Content = new NodePropertiesEditors.ActionNodePropertiesEditor((ActionNodeConfiguration)selectedNode, this);
            } else if (selectedNode is CbrNodeConfiguration) {
                propertiesContainer.Content = new NodePropertiesEditors.CbrNodePropertiesEditor((CbrNodeConfiguration)selectedNode, this);
            } else if (selectedNode is TerminatorNodeConfiguration) {
                propertiesContainer.Content = new NodePropertiesEditors.TerminatorNodePropertiesEditor((TerminatorNodeConfiguration)selectedNode, this);
            }

            SelectedNode = selectedNode;
            NodeSelectedEventArgs nodeSelectedEventArgs = new NodeSelectedEventArgs(originalSelectedNode, selectedNode);
            NodeSelected(this, nodeSelectedEventArgs);
        }
    }
}
