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

            MenuItem menuItemAddActionNode = new MenuItem { Header = new Label { Content = "Action", Margin = new Thickness(0, 0, 0, 0) } }; ;
            menuItemAddActionNode.Click += delegate {
                AddNode(result, delegate { return new ActionNodeConfiguration(); });
            };

            MenuItem menuItemAddCbrNode = new MenuItem { Header = new Label { Content = "CBR", Margin = new Thickness(0, 0, 0, 0) } };
            menuItemAddCbrNode.Click += delegate {
                AddNode(result, delegate { return new CbrNodeConfiguration(); });
            };

            MenuItem menuItemAddTerminatorNode = new MenuItem { Header = new Label { Content = "Terminator", Margin = new Thickness(0, 0, 0, 0) } };
            menuItemAddTerminatorNode.Click += delegate {
                Point menuLocationOnCanvas = result.TranslatePoint(new Point(), graphCanvas.Canvas);
                AddNode(result, delegate { return new TerminatorNodeConfiguration(); });
            };

            MenuItem menuItemAdd = new MenuItem { Header = "Add node..." };
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
