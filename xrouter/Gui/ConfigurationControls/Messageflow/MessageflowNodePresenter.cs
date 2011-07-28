using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleDiagrammer;
using XRouter.Common.MessageFlowConfig;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace XRouter.Gui.ConfigurationControls.Messageflow
{
    class MessageflowNodePresenter : NodePresenter<NodeConfiguration>
    {
        private NodeConfiguration node;
        private MessageflowGraphPresenter messageflowGraphPresenter;
        private NodeSelectionManager nodeSelectionManager;

        public override FrameworkElement DragArea { get { return nodeBorder; } }

        private Border nodeBorder;
        private TextBlock uiName;

        public override Point Location {
            get { return node.Location; }
            set { node.Location = value; }
        }

        public MessageflowNodePresenter(NodeConfiguration node, MessageflowGraphPresenter messageflowGraphPresenter, NodeSelectionManager nodeSelectionManager)
            : base(node)
        {
            this.node = node;
            this.messageflowGraphPresenter = messageflowGraphPresenter;
            this.nodeSelectionManager = nodeSelectionManager;

            Image uiIcon = new Image {
                Margin = new Thickness(2, 2, 6, 0),
                Height = 24
            };
            Grid.SetRow(uiIcon, 0);
            Grid.SetColumn(uiIcon, 0);

            TextBlock uiType = new TextBlock {
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 11
            };
            Grid.SetRow(uiType, 0);
            Grid.SetColumn(uiType, 1);

            uiName = new TextBlock {
                Text = node.Name,
                Foreground = Brushes.Black,
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5, 2, 3, 2)
            };
            Grid.SetRow(uiName, 1);
            Grid.SetColumn(uiName, 0);
            Grid.SetColumnSpan(uiName, 3);

            if (node is CbrNodeConfiguration) {
                uiType.Text = "CBR";
                uiIcon.Source = new BitmapImage(new Uri("pack://application:,,,/XRouter.Gui;component/Resources/OrgChartHS.png"));
            } else if (node is ActionNodeConfiguration) {
                uiType.Text = "Action";
                uiIcon.Source = new BitmapImage(new Uri("pack://application:,,,/XRouter.Gui;component/Resources/Generic_Device.png"));
            } else if (node is TerminatorNodeConfiguration) {
                uiType.Text = "Terminator";
                uiIcon.Source = new BitmapImage(new Uri("pack://application:,,,/XRouter.Gui;component/Resources/1446_envelope_stamp_clsd_32.png"));
            }

            nodeBorder = new Border {
                Child = new Grid {
                    RowDefinitions = {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
                    },
                    ColumnDefinitions = {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    },
                    Children = {
                        uiIcon,
                        uiType,
                        uiName
                    }
                },
                Padding = new Thickness(5, 3, 5, 5),
                CornerRadius = new CornerRadius(5),
                Effect = new DropShadowEffect { Opacity = 0.6, ShadowDepth = 8 }
            };

            nodeBorder.PreviewMouseDown += delegate {
                nodeSelectionManager.SelectNode(node);
            };
            nodeBorder.ContextMenu = CreateNodeContextMenu();

            Content = nodeBorder;

            UpdateNodeSelection();
            nodeSelectionManager.NodeSelected += delegate { UpdateNodeSelection(); };

            node.NameChanged += delegate {
                uiName.Text = node.Name;
            };
        }

        private ContextMenu CreateNodeContextMenu()
        {
            ContextMenu result = new ContextMenu();

            MenuItem menuItemRemove = new MenuItem {
                Icon = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/XRouter.Gui;component/Resources/delete.png")), Height = 18 },
                Header = new TextBlock { Text = "Remove", FontSize = 14 }
            };
            menuItemRemove.Click += delegate {
                messageflowGraphPresenter.Messageflow.RemoveNode(node);
                messageflowGraphPresenter.RaiseGraphChanged();
                nodeSelectionManager.SelectNode(null);
            };

            result.Items.Add(menuItemRemove);
            return result;
        }

        private void UpdateNodeSelection()
        {
            bool isSelected = nodeSelectionManager.SelectedNode == node;
            if (isSelected) {
                nodeBorder.Background = new LinearGradientBrush {
                    GradientStops = {
                        new GradientStop( Colors.CornflowerBlue, -0),
                        new GradientStop(Colors.White, 1.4),
                    }
                };
                nodeBorder.BorderBrush = Brushes.Black;
                nodeBorder.BorderThickness = new Thickness(2);
                nodeBorder.LayoutTransform = new ScaleTransform(1.15, 1.15);
            } else {
                nodeBorder.Background = new LinearGradientBrush {
                    GradientStops = {
                        new GradientStop( Colors.LightSkyBlue, 0),
                        new GradientStop(Colors.White, 1.4),
                    }
                };
                nodeBorder.BorderBrush = Brushes.Black;
                nodeBorder.BorderThickness = new Thickness(1);
                nodeBorder.LayoutTransform = null;
            }
        }
    }
}
