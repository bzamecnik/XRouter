using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleDiagrammer;
using XRouter.Common.MessageFlowConfig;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace XRouter.Gui.ConfigurationControls.Messageflow
{
    class MessageflowNodePresenter : NodePresenter<NodeConfiguration>
    {
        private NodeConfiguration node;
        private MessageflowGraphPresenter messageflowGraphPresenter;
        private NodeSelectionManager propertiesManager;

        public override FrameworkElement DragArea { get { return nodeBorder; } }

        private Border nodeBorder;
        private Label nodeLabel;

        public override Point Location {
            get { return node.Location; }
            set { node.Location = value; }
        }

        public MessageflowNodePresenter(NodeConfiguration node, MessageflowGraphPresenter messageflowGraphPresenter, NodeSelectionManager propertiesManager)
            : base(node)
        {
            this.node = node;
            this.messageflowGraphPresenter = messageflowGraphPresenter;
            this.propertiesManager = propertiesManager;

            nodeLabel = new Label {
                Content = node.Name,
                Foreground = Brushes.Black
            };
            nodeBorder = new Border {
                Child = nodeLabel,
                Padding = new Thickness(5),
                CornerRadius = new CornerRadius(5)
            };

            nodeBorder.PreviewMouseDown += delegate {
                propertiesManager.SelectNode(node);
            };
            nodeBorder.ContextMenu = CreateNodeContextMenu();

            Content = nodeBorder;

            UpdateNodeSelection();
            propertiesManager.NodeSelected += delegate { UpdateNodeSelection(); };

            node.NameChanged += delegate {
                nodeLabel.Content = node.Name;
            };
        }

        private ContextMenu CreateNodeContextMenu()
        {
            ContextMenu result = new ContextMenu();

            MenuItem menuItemRemove = new MenuItem { Header = "Remove" };
            menuItemRemove.Click += delegate {
                messageflowGraphPresenter.Messageflow.RemoveNode(node);
                messageflowGraphPresenter.RaiseGraphChanged();
            };

            result.Items.Add(menuItemRemove);
            return result;
        }

        private void UpdateNodeSelection()
        {
            Color mainColor = Colors.LightSalmon;
            if (node is CbrNodeConfiguration) {
                mainColor = Colors.LightBlue;
            } else if (node is TerminatorNodeConfiguration) {
                mainColor = Colors.LightGray;
            }

            bool isSelected = propertiesManager.SelectedNode == node;
            if (isSelected) {
                nodeBorder.Background = new LinearGradientBrush {
                    GradientStops = {
                        new GradientStop(mainColor, 0),
                        new GradientStop(Colors.Black, 3),
                    }
                };
                nodeBorder.BorderBrush = Brushes.Black;
                nodeBorder.BorderThickness = new Thickness(2);
                nodeLabel.FontWeight = FontWeights.Bold;
            } else {
                nodeBorder.Background = new LinearGradientBrush {
                    GradientStops = {
                        new GradientStop(mainColor, 0),
                        new GradientStop(Colors.White, 1.5),
                    }
                };
                nodeBorder.BorderBrush = Brushes.Black;
                nodeBorder.BorderThickness = new Thickness(1);
                nodeLabel.FontWeight = FontWeights.Normal;
            }
        }
    }
}
