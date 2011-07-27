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
    /// Interaction logic for ActionNodePropertiesEditor.xaml
    /// </summary>
    public partial class ActionNodePropertiesEditor : UserControl
    {
        private ActionNodeConfiguration node;
        private NodeSelectionManager nodeSelectionManager;

        private ActionConfiguration currentAction;
        private ConfigurationEditor currentConfigurationEditor;

        internal ActionNodePropertiesEditor(ActionNodeConfiguration node, NodeSelectionManager nodeSelectionManager)
        {
            InitializeComponent();
            this.node = node;
            this.nodeSelectionManager = nodeSelectionManager;

            uiName.Text = node.Name;

            currentAction = node.Actions.First();
            currentConfigurationEditor = Configurator.CreateEditor(currentAction.ConfigurationMetadata);
            currentConfigurationEditor.LoadConfiguration(currentAction.Configuration.XDocument);

            uiActionConfigurationContainer.Child = currentConfigurationEditor;
        }

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

        private void uiSelectNextNode_Click(object sender, RoutedEventArgs e)
        {
            if (uiSelectNextNode.IsChecked == true) {
                nodeSelectionManager.NodeSelecting += SelectNodeAsNextTarget;
            } else {
                nodeSelectionManager.NodeSelecting -= SelectNodeAsNextTarget;
            }
        }

        private void SelectNodeAsNextTarget(object sender, NodeSelectingEventArgs e)
        {
            e.IsCancelled = true;
            uiSelectNextNode.IsChecked = false;
            nodeSelectionManager.NodeSelecting -= SelectNodeAsNextTarget;

            node.NextNode = e.NewSelectedNode;
            nodeSelectionManager.MessageflowGraphPresenter.RaiseGraphChanged();
        }
    }
}
