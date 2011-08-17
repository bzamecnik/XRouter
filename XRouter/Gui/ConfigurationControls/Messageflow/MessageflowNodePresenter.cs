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
using System.Windows.Shapes;

namespace XRouter.Gui.ConfigurationControls.Messageflow
{
    class MessageflowNodePresenter : NodePresenter<NodeConfiguration>
    {
        private NodeConfiguration node;
        private MessageflowGraphPresenter messageflowGraphPresenter;
        private NodeSelectionManager nodeSelectionManager;

        public override FrameworkElement DragArea { get { return (FrameworkElement)Content; } }

        private Shape nodeShape;
        private TextBlock uiName;
        private Grid uiNode;

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
                Margin = new Thickness(5, 2, 3, 2),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetRow(uiName, 1);
            Grid.SetColumn(uiName, 0);
            Grid.SetColumnSpan(uiName, 3);

            Thickness nodeContentPadding = new Thickness(10);
            Brush nodeBackground = null;
            if (node is CbrNodeConfiguration) {
                nodeContentPadding = new Thickness(30, 10, 30, 10);
                nodeBackground = new RadialGradientBrush {
                    GradientStops = {
                        new GradientStop(Colors.Black, 0),
                        new GradientStop(Colors.Transparent, 1.4)
                    }
                };
                nodeShape = new Polygon {
                    Points = { new Point(0, 5), new Point(50, 0), new Point(100, 5), new Point(50, 10) },
                };
                uiType.Text = "CBR";
                uiIcon.Source = new BitmapImage(new Uri("pack://application:,,,/XRouter.Gui;component/Resources/nfs-unmount-icon.png"));
            } else if (node is ActionNodeConfiguration) {
                nodeShape = new Rectangle {
                    RadiusX = 1, RadiusY = 1
                };
                uiType.Text = "Action";
                uiIcon.Source = new BitmapImage(new Uri("pack://application:,,,/XRouter.Gui;component/Resources/Actions-tool-animator-icon.png"));
            } else if (node is TerminatorNodeConfiguration) {
                nodeShape = new Ellipse {
                };
                uiType.Text = "Terminator";
                uiIcon.Source = new BitmapImage(new Uri("pack://application:,,,/XRouter.Gui;component/Resources/Button-exit-icon.png"));
            }
            nodeShape.Stretch = Stretch.Fill;
            nodeShape.Effect = new DropShadowEffect { Opacity = 0.6, ShadowDepth = 8 };

            uiNode = new Grid {
                Background = nodeBackground,
                Children = {
                    nodeShape,
                    new Border {
                        Padding = nodeContentPadding,
                        Child = uiName
                    }
                    //new Grid {
                    //    HorizontalAlignment = HorizontalAlignment.Center,
                    //    VerticalAlignment = VerticalAlignment.Center,
                    //    RowDefinitions = {
                    //        new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    //        new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
                    //    },
                    //    ColumnDefinitions = {
                    //        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    //        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    //        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    //    },
                    //    Children = {
                    //        uiIcon,
                    //        uiType,
                    //        uiName
                    //    }
                    //}
                }
            };
            uiNode.PreviewMouseDown += delegate {
                nodeSelectionManager.SelectNode(node);
            };
            uiNode.ContextMenu = CreateNodeContextMenu();

            UpdateNodeSelection();
            nodeSelectionManager.NodeSelected += delegate { UpdateNodeSelection(); };

            node.NameChanged += delegate {
                uiName.Text = node.Name;
            };

            Content = uiNode;
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
                nodeShape.Fill = new LinearGradientBrush {
                    GradientStops = {
                        new GradientStop( Colors.CornflowerBlue, 0),
                        new GradientStop(Colors.White, 1.4),
                    }
                };
                nodeShape.Stroke = Brushes.Black;
                nodeShape.StrokeThickness = 2;
                uiNode.LayoutTransform = new ScaleTransform(1.15, 1.15);
            } else {
                nodeShape.Fill = new LinearGradientBrush {
                    GradientStops = {
                        new GradientStop( Colors.LightSkyBlue, 0),
                        new GradientStop(Colors.White, 1.4),
                    }
                };
                nodeShape.Stroke = Brushes.Black;
                nodeShape.StrokeThickness = 1;
                uiNode.LayoutTransform = null;
            }
        }
    }
}
