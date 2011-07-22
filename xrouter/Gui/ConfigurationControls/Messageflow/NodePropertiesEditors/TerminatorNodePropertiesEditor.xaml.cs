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
            if (node.IsReturningOutput) {
                uiResultMessage.Text = node.ResultMessageSelection.SelectionPattern;
            } else {
                uiResultMessage.IsEnabled = false;
            }
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

        private void uiResultMessage_LostFocus(object sender, RoutedEventArgs e)
        {
            node.ResultMessageSelection = new TokenSelection(uiResultMessage.Text);
        }

        private void uiResultMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                node.ResultMessageSelection = new TokenSelection(uiResultMessage.Text);
            }
            if (e.Key == Key.Escape) {
                uiResultMessage.Text = node.ResultMessageSelection.SelectionPattern;
            }
        }

        private void uiReturnsResult_Click(object sender, RoutedEventArgs e)
        {
            if (uiIsReturningOutput.IsChecked == true) {
                node.IsReturningOutput = true;
                node.ResultMessageSelection = new TokenSelection(uiResultMessage.Text);
                uiResultMessage.IsEnabled = true;
            } else {
                node.IsReturningOutput = false;
                uiResultMessage.IsEnabled = false;
            }
        }
    }
}
