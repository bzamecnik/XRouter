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

namespace XRouter.Gui.ConfigurationControls.Messageflow.NodePropertiesEditors
{
    /// <summary>
    /// Interaction logic for TerminatorNodePropertiesEditor.xaml
    /// </summary>
    public partial class TerminatorNodePropertiesEditor : UserControl
    {
        private TerminatorNodeConfiguration node;
        private NodeSelectionManager nodeSelectionManager;

        internal TerminatorNodePropertiesEditor(TerminatorNodeConfiguration node, NodeSelectionManager nodeSelectionManager)
        {
            InitializeComponent();
            this.node = node;
            this.nodeSelectionManager = nodeSelectionManager;

            uiName.Text = node.Name;
            uiIsReturningOutput.IsChecked = node.IsReturningOutput;
            uiResultMessage.IsEnabled = node.IsReturningOutput;

            if (node.ResultMessageSelection == null) {
                node.ResultMessageSelection = new TokenSelection();
            }
            uiResultMessage.Selection = node.ResultMessageSelection;
        }

        #region Name editing

        private void uiName_LostFocus(object sender, RoutedEventArgs e)
        {
            nodeSelectionManager.RenameNode(node, uiName);
        }

        private void uiName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                nodeSelectionManager.RenameNode(node, uiName);
            }
            if (e.Key == Key.Escape) {
                uiName.Text = node.Name;
            }
        }

        #endregion

        private void uiReturnsResult_Click(object sender, RoutedEventArgs e)
        {
            if (uiIsReturningOutput.IsChecked == true) {
                node.IsReturningOutput = true;
                uiResultMessage.IsEnabled = true;
            } else {
                node.IsReturningOutput = false;
                uiResultMessage.IsEnabled = false;
            }
        }
    }
}
