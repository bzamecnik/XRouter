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
using XRouter.Common;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common.MessageFlowConfig;
using SimpleDiagrammer;
using System.Threading.Tasks;
using System.Threading;

namespace XRouter.Gui.ConfigurationControls.Messageflow
{
    /// <summary>
    /// Interaction logic for MessageflowConfigurationControl.xaml
    /// </summary>
    public partial class MessageflowConfigurationControl : UserControl, IConfigurationControl
    {
        public bool IsDirty
        {
            get { throw new NotImplementedException(); }
        }

        private IBrokerServiceForManagement brokerService;
        private ConfigurationTree configTreeNode;
        private ApplicationConfiguration appConfig;

        internal MessageFlowConfiguration Messageflow { get; private set; }
        internal NodeSelectionManager NodeSelectionManager { get; private set; }
        internal MessageflowGraphPresenter MessageflowGraphPresenter { get; private set; }

        private GraphCanvas graphCanvas;

        public MessageflowConfigurationControl()
        {
            InitializeComponent();
        }

        public void Initialize(ApplicationConfiguration appConfig, IBrokerServiceForManagement brokerService, ConfigurationTree configTreeNode)
        {
            this.appConfig = appConfig;
            this.brokerService = brokerService;
            this.configTreeNode = configTreeNode;

            NodeSelectionManager = new NodeSelectionManager(uiNodePropertiesContainer);

            Messageflow = appConfig.GetMessageFlow(appConfig.GetCurrentMessageFlowGuid());
            MessageflowGraphPresenter = new MessageflowGraphPresenter(Messageflow, NodeSelectionManager);
            NodeSelectionManager.MessageflowGraphPresenter = MessageflowGraphPresenter;
            
            graphCanvas = MessageflowGraphPresenter.CreateGraphCanvas();
            uiDesignerContainer.Child = graphCanvas;
            uiDesignerContainer.ContextMenu = CreateGraphCanvasContextMenu();
        }

        private ContextMenu CreateGraphCanvasContextMenu()
        {
            ContextMenu result = new ContextMenu();

            MenuItem menuItemAddActionNode = new MenuItem {
                Header = new StackPanel {
                    Margin = new Thickness(-10, 2, 2, 2),
                    VerticalAlignment = VerticalAlignment.Center,
                    Orientation = Orientation.Horizontal,
                    Children = { 
                        new Image {
                            Source = new BitmapImage(new Uri("pack://application:,,,/XRouter.Gui;component/Resources/Generic_Device.png")),
                            Margin = new Thickness(0, 0, 5, 0),
                            Height = 20
                        },
                        new TextBlock { Text = "Action", FontSize = 14, FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Center }
                    }
                }
            };
            menuItemAddActionNode.Click += delegate {
                AddNode(result, delegate { return new ActionNodeConfiguration(); });
            };

            MenuItem menuItemAddCbrNode = new MenuItem {
                Header = new StackPanel {
                    Margin = new Thickness(-10, 2, 2, 2),
                    VerticalAlignment = VerticalAlignment.Center,
                    Orientation = Orientation.Horizontal,
                    Children = { 
                        new Image {
                            Source = new BitmapImage(new Uri("pack://application:,,,/XRouter.Gui;component/Resources/OrgChartHS.png")),
                            Margin = new Thickness(0, 0, 5, 0),
                            Height = 20
                        },
                        new TextBlock { Text = "CBR", FontSize = 14, FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Center }
                    }
                }
            };
            menuItemAddCbrNode.Click += delegate {
                AddNode(result, delegate { return new CbrNodeConfiguration(); });
            };

            MenuItem menuItemAddTerminatorNode = new MenuItem {
                Header = new StackPanel {
                    Margin = new Thickness(-10, 2, 2, 2),
                    VerticalAlignment = VerticalAlignment.Center,
                    Orientation = Orientation.Horizontal,
                    Children = { 
                        new Image {
                            Source = new BitmapImage(new Uri("pack://application:,,,/XRouter.Gui;component/Resources/1446_envelope_stamp_clsd_32.png")),
                            Margin = new Thickness(0, 0, 5, 0),
                            Height = 20
                        },
                        new TextBlock { Text = "Terminator", FontSize = 14, FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Center }
                    }
                } 
            };
            menuItemAddTerminatorNode.Click += delegate {
                Point menuLocationOnCanvas = result.TranslatePoint(new Point(), graphCanvas.Canvas);
                AddNode(result, delegate { return new TerminatorNodeConfiguration(); });
            };

            MenuItem menuItemAdd = new MenuItem {
                Header = new TextBlock { Text = "Add node...", FontSize = 14 }  
            };
            menuItemAdd.Items.Add(menuItemAddActionNode);
            menuItemAdd.Items.Add(menuItemAddCbrNode);
            menuItemAdd.Items.Add(menuItemAddTerminatorNode);

            result.Items.Add(menuItemAdd);
            return result;
        }

        private void AddNode(ContextMenu menu, Func<NodeConfiguration> nodeFactory)
        {
            NodeConfiguration node = nodeFactory();
            node.Name = "New node";
            Point menuLocationOnCanvas = menu.TranslatePoint(new Point(), graphCanvas.Canvas);
            node.Location = menuLocationOnCanvas - graphCanvas.CanvasLocationOffset;
            Messageflow.Nodes.Add(node);
            MessageflowGraphPresenter.RaiseGraphChanged();

            Task.Factory.StartNew(delegate {
                Thread.Sleep(500);
                Dispatcher.Invoke(new Action(delegate {
                    NodeSelectionManager.SelectNode(node);
                }));
            });
        }

        public void Save()
        {
            Messageflow.PromoteToNewVersion();
            appConfig.AddMessageFlow(Messageflow);
            appConfig.SetCurrentMessageFlowGuid(Messageflow.Guid);
        }

        public void Clear()
        {
        }
    }
}
