using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using SimpleDiagrammer;
using XRouter.Common;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common.MessageFlowConfig;
using XRouter.Gui.Utils;
using XRouter.Manager;

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

        private IConsoleServer consoleServer;
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

        public void Initialize(ApplicationConfiguration appConfig, IConsoleServer consoleServer, ConfigurationTree configTreeNode)
        {
            this.appConfig = appConfig;
            this.consoleServer = consoleServer;
            this.configTreeNode = configTreeNode;

            NodeSelectionManager = new NodeSelectionManager(uiNodePropertiesContainer, appConfig);

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
                Icon = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/XRouter.Gui;component/Resources/Actions-tool-animator-icon.png")), Height = 20 },
                Header = new TextBlock { Text = "Action", FontSize = 14, FontWeight = FontWeights.Bold }
            };
            menuItemAddActionNode.Click += delegate {
                AddNode(result, delegate { return new ActionNodeConfiguration(); });
            };

            MenuItem menuItemAddCbrNode = new MenuItem {
                Icon = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/XRouter.Gui;component/Resources/nfs-unmount-icon.png")), Height = 20 },
                Header = new TextBlock { Text = "CBR", FontSize = 14, FontWeight = FontWeights.Bold }
            };
            menuItemAddCbrNode.Click += delegate {
                AddNode(result, delegate { return new CbrNodeConfiguration(); });
            };

            MenuItem menuItemAddTerminatorNode = new MenuItem {
                Icon = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/XRouter.Gui;component/Resources/Button-exit-icon.png")), Height = 20 },
                Header = new TextBlock { Text = "Terminator", FontSize = 14, FontWeight = FontWeights.Bold }
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

            ThreadUtils.InvokeLater(TimeSpan.FromSeconds(0.5), delegate {
                NodeSelectionManager.SelectNode(node);
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

        private void uiImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Messageflow configuration|*.xmf";
            dialog.DefaultExt = ".xmf";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == true) {
                using (var fs = new FileStream(dialog.FileName, FileMode.Open)) {
                    Messageflow = MessageFlowConfiguration.Read(fs);
                }
                NodeSelectionManager = new NodeSelectionManager(uiNodePropertiesContainer, appConfig);
                uiNodePropertiesContainer.Child = null;
                MessageflowGraphPresenter = new MessageflowGraphPresenter(Messageflow, NodeSelectionManager);
                NodeSelectionManager.MessageflowGraphPresenter = MessageflowGraphPresenter;
                graphCanvas = MessageflowGraphPresenter.CreateGraphCanvas();
                uiDesignerContainer.Child = graphCanvas;
            }
        }

        private void uiExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Messageflow configuration|*.xmf";
            dialog.DefaultExt = ".xmf";
            dialog.OverwritePrompt = true;
            if (dialog.ShowDialog() == true) {
                using (var fs = new FileStream(dialog.FileName, FileMode.Create)) {
                    Messageflow.Write(fs);
                }
            }
        }
    }
}
