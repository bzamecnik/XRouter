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
using System.Xml.Linq;
using ObjectConfigurator;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Xrm;
using XRouter.Gui.CommonControls;
using XRouter.Common;

namespace XRouter.Gui
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        private ConfigurationManager ConfigManager { get; set; }

        private ConfigurationTreeItem currentConfigurationTreeItem;
        private IConfigurationControl currentConfigurationControl;

		public MainWindow()
        {
            Configurator.CustomItemTypes.Add(new TokenSelectionConfigurationItemType(() => new TokenSelectionEditor()));
            Configurator.CustomItemTypes.Add(new XrmUriConfigurationItemType(() => new XrmUriEditor()));
            Configurator.CustomItemTypes.Add(new UriConfigurationItemType(() => new UriEditor()));

            #region Run xrouter server

            bool isXRouterRunning = System.Diagnostics.Process.GetProcessesByName("DaemonNT").Length > 0;
            if (!isXRouterRunning)
            {
                string binPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string daemonNTPath = System.IO.Path.Combine(binPath, @"DaemonNT.exe");
                var serverProcesss = new System.Diagnostics.ProcessStartInfo(daemonNTPath, "debug xroutermanager");
                serverProcesss.WorkingDirectory = System.IO.Path.GetDirectoryName(daemonNTPath);
                System.Diagnostics.Process.Start(serverProcesss);
                System.Threading.Thread.Sleep(2000);
            }
            #endregion

            InitializeComponent();
		}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigManager = new ConfigurationManager();

            ReloadConfigurationFromServer();

            uiTokens.Initialize(ConfigManager);
            uiTraceLog.Initialize(delegate(DateTime minDate, DateTime maxDate, LogLevelFilters levelFilter, int pageNumber, int pageSize) {
                var logs = ConfigManager.ConsoleServer.GetTraceLogEntries(minDate, maxDate, levelFilter, pageSize, pageNumber);
                var rows = logs.Select(l => new LogViewControl.LogRow(l.Created, l.LogLevel, l.XmlContent)).ToArray();
                return rows;
            });
            uiEventLog.Initialize(delegate(DateTime minDate, DateTime maxDate, LogLevelFilters levelFilter, int pageNumber, int pageSize) {
                var logs = ConfigManager.ConsoleServer.GetEventLogEntries(minDate, maxDate, levelFilter, pageSize, pageNumber);
                var rows = logs.Select(l => new LogViewControl.LogRow(l.Created, l.LogLevel, l.Message)).ToArray();
                return rows;
            });
        }

        private TreeViewItem CreateUIItem(ConfigurationTreeItem item)
        {
            TreeViewItem uiItem = new TreeViewItem {
                Tag = item,
                Header = item.Name
            };
            foreach (var child in item.Children) {
                TreeViewItem uiChild = CreateUIItem(child);
                uiItem.Items.Add(uiChild);
            }
            return uiItem;
        }

        private void uiConfigurationTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            uiConfigurationContainer.Content = null;
            currentConfigurationControl = null;
            if (e.NewValue == null) {
                return;
            }

            currentConfigurationTreeItem = (ConfigurationTreeItem)((TreeViewItem)e.NewValue).Tag;
            currentConfigurationControl = currentConfigurationTreeItem.ConfigurationControl;
            uiConfigurationContainer.Content = currentConfigurationControl;
        }

        private void LoadFromServer_Click(object sender, RoutedEventArgs e)
        {
            ReloadConfigurationFromServer();
        }

        private void SaveToServer_Click(object sender, RoutedEventArgs e)
		{
            var items = uiConfigurationTree.Items.OfType<TreeViewItem>().Select(i => (ConfigurationTreeItem)i.Tag);
            foreach (ConfigurationTreeItem item in items) {
                item.SaveRecursively();
            }
            ConfigManager.SaveConfiguration();
		}

        private void ReloadConfigurationFromServer()
        {
            ConfigManager.LoadConfigurationFromServer();
            ReloadConfigurationTree();
        }

        private void ReloadConfigurationTree()
        {
            uiConfigurationTree.Items.Clear();
            ConfigurationTreeItem root = ConfigManager.CreateConfigurationTreeRoot();
            foreach (var item in root.Children) {
                TreeViewItem uiItem = CreateUIItem(item);
                uiConfigurationTree.Items.Add(uiItem);
            }
        }
	}
}
