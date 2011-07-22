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
using XRouter.Common.Xrm;

namespace XRouter.Gui.ConfigurationControls.Messageflow.NodePropertiesEditors
{
    /// <summary>
    /// Interaction logic for CbrNodePropertiesEditor.xaml
    /// </summary>
    public partial class CbrNodePropertiesEditor : UserControl
    {
        private CbrNodeConfiguration node;
        private NodeSelectionManager nodeSelectionManager;

        internal CbrNodePropertiesEditor(CbrNodeConfiguration node, NodeSelectionManager nodeSelectionManager)
        {
            InitializeComponent();
            this.node = node;
            this.nodeSelectionManager = nodeSelectionManager;

            uiName.Text = node.Name;
            uiTestedMessage.Text = node.TestedSelection.SelectionPattern;
            foreach (XrmUri branchKey in node.Branches.Keys) {
                AddBranchToEditor(branchKey);
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

        private void uiTestedMessage_LostFocus(object sender, RoutedEventArgs e)
        {
            node.TestedSelection = new TokenSelection(uiTestedMessage.Text);
        }

        private void uiTestedMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                node.TestedSelection = new TokenSelection(uiTestedMessage.Text);
            }
            if (e.Key == Key.Escape) {
                uiTestedMessage.Text = node.TestedSelection.SelectionPattern;
            }
        }

        private void uiSelectDefaultTarget_Click(object sender, RoutedEventArgs e)
        {
            if (uiSelectDefaultTarget.IsChecked == true) {
                nodeSelectionManager.NodeSelecting += SelectNodeAsDefaultTarget;
            } else {
                nodeSelectionManager.NodeSelecting -= SelectNodeAsDefaultTarget;
            }
        }

        private void SelectNodeAsDefaultTarget(object sender, NodeSelectingEventArgs e)
        {
            e.IsCancelled = true;
            uiSelectDefaultTarget.IsChecked = false;
            nodeSelectionManager.NodeSelecting -= SelectNodeAsDefaultTarget;

            node.DefaultTarget = e.NewSelectedNode;
            nodeSelectionManager.MessageflowGraphPresenter.RaiseGraphChanged();
        }

        #region Editing branches

        private void uiAddBranch_Click(object sender, RoutedEventArgs e)
        {
            XrmUri branchKey = new XrmUri(string.Empty);
            node.Branches.Add(branchKey, null);
            AddBranchToEditor(branchKey);
        }

        private void AddBranchToEditor(XrmUri branchKey)
        {
            uiBranches.RowDefinitions.Add(new RowDefinition());
            int rowNumber = uiBranches.RowDefinitions.Count - 1;

            TextBox uiCondition = new TextBox {
                Text = branchKey.XPath,
                Margin = new Thickness(5, 2, 5, 2)
            };
            Grid.SetRow(uiCondition, rowNumber);
            Grid.SetColumn(uiCondition, 0);

            Button uiSelectTarget = new Button {
                Content = "Select",
                Margin = new Thickness(5, 2, 5, 2),
                Padding = new Thickness(10, 2, 10, 2),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetRow(uiSelectTarget, rowNumber);
            Grid.SetColumn(uiSelectTarget, 1);

            Button uiRemove = new Button {
                Content = "X",
                Margin = new Thickness(5, 2, 5, 2),
                Padding = new Thickness(10, 2, 10, 2),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetRow(uiRemove, rowNumber);
            Grid.SetColumn(uiRemove, 2);
            uiRemove.Click += delegate {
                uiBranches.Children.Remove(uiCondition);
                uiBranches.Children.Remove(uiSelectTarget);
                uiBranches.Children.Remove(uiRemove);
                uiBranches.RowDefinitions.RemoveAt(uiBranches.RowDefinitions.Count - 1);
                node.Branches.Remove(branchKey);
            };

            uiBranches.Children.Add(uiCondition);
            uiBranches.Children.Add(uiSelectTarget);
            uiBranches.Children.Add(uiRemove);
        }

        #endregion
    }
}
