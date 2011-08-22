using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XRouter.Common.MessageFlowConfig;
using ObjectConfigurator;

namespace XRouter.Gui.ConfigurationControls.Messageflow.NodePropertiesEditors
{
    /// <summary>
    /// Interaction logic for EntryNodePropertiesEditor.xaml
    /// </summary>
    public partial class EntryNodePropertiesEditor : UserControl
    {
        private EntryNodeConfiguration node;
        private NodeSelectionManager nodeSelectionManager;

        private ActionConfiguration activeAction;
        private ConfigurationEditor activeConfigurationEditor;

        internal EntryNodePropertiesEditor(EntryNodeConfiguration node, NodeSelectionManager nodeSelectionManager)
        {
            InitializeComponent();
            this.node = node;
            this.nodeSelectionManager = nodeSelectionManager;

            uiName.Text = node.Name;

            #region Prepare next node selector
            uiNextNodeSelector.Initialize(nodeSelectionManager, node, () => node.NextNode, delegate(NodeConfiguration nextNode) {
                node.NextNode = nextNode;
            });
            #endregion
        }

        #region Name editing

        private void uiName_LostFocus(object sender, RoutedEventArgs e)
        {
            node.Name = uiName.Text;
        }

        private void uiName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                node.Name = uiName.Text;
            }
            if (e.Key == Key.Escape) {
                uiName.Text = node.Name;
            }
        }

        #endregion
    }
}
