﻿using System;
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
using XRouter.Gui.CommonControls;
using XRouter.Processor;
using System.Xml.Linq;

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

            if (node.TestedSelection == null) {
                node.TestedSelection = new TokenSelection();
            }
            uiTestedMessage.Selection = node.TestedSelection;

            uiDefaultTargetSelector.Initialize(nodeSelectionManager, node, () => node.DefaultTarget, delegate(NodeConfiguration defaultTarget) {
                node.DefaultTarget = defaultTarget;
            });

            #region Prepare branches
            uiBranches.Initialize(delegate {
                XrmUri branchKey = new XrmUri();
                node.Branches.Add(branchKey, null);
                return CreateBranchPresentation(branchKey);
            });
            uiBranches.ItemRemoved += delegate(FrameworkElement uiBranch) {
                XrmUri branchKey = (XrmUri)uiBranch.Tag;
                node.Branches.Remove(branchKey);
                nodeSelectionManager.MessageflowGraphPresenter.RaiseGraphChanged();
            };
            uiBranches.ItemAdded += delegate(FrameworkElement uiBranch) { 
                nodeSelectionManager.MessageflowGraphPresenter.RaiseGraphChanged(); 
            };
            foreach (XrmUri branchKey in node.Branches.Keys) {
                uiBranches.AddItem(CreateBranchPresentation(branchKey));
            }
            #endregion

            #region Prepare uiTestXml
            uiTestXml.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("XML");
            uiTestXml.Options.ConvertTabsToSpaces = true;
            uiTestXml.Options.IndentationSize = 4;
            uiTestXml.ShowLineNumbers = true;
            #endregion
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

        #region Branches editing

        private FrameworkElement CreateBranchPresentation(XrmUri branchKey)
        {
            XrmUriEditor uiCondition = new XrmUriEditor(nodeSelectionManager.ConfigManager) {
                XrmUri = branchKey,
                Margin = new Thickness(5, 2, 5, 2)
            };
            Grid.SetColumn(uiCondition, 0);

            TargetNodeSelector uiTargetSelector = new TargetNodeSelector {
                Margin = new Thickness(5, 2, 10, 2),
            };
            uiTargetSelector.Initialize(nodeSelectionManager, node, () => node.Branches[branchKey], delegate(NodeConfiguration targetNode) {
                node.Branches[branchKey] = targetNode;
            });
            Grid.SetColumn(uiTargetSelector, 1);

            Grid result = new Grid {
                Tag = branchKey,
                ColumnDefinitions = {
                    new ColumnDefinition { Width=new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width=new GridLength(1, GridUnitType.Auto) }
                },
                Children = { uiCondition, uiTargetSelector }
            };
            return result;
        }

        #endregion

        private void TestXml_Click(object sender, RoutedEventArgs e)
        {
            XDocument xDoc;
            try {
                xDoc = XDocument.Parse(uiTestXml.Text);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string targetNodeName;
            try {
                CbrEvaluator evaluator = new CbrEvaluator(node, GetXrmResource);
                targetNodeName = evaluator.GetTargetNodeName(xDoc);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string message = string.Format("Target node name: {0}", targetNodeName);
            MessageBox.Show(message, "CBR result", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private XDocument GetXrmResource(XrmUri xrmUri)
        {
            XDocument xrmContent = nodeSelectionManager.AppConfig.GetXrmContent();
            XDocument result = xrmUri.GetResource(xrmContent);
            return result;
        }
    }
}
