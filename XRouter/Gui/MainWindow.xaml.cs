using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Microsoft.Win32;
using ObjectConfigurator;
using XRouter.Common;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Xrm;
using XRouter.Gui.CommonControls;
using XRouter.Gui.Xrm;
using System.Windows.Input;
using System.ServiceProcess;
using System.Windows.Threading;

namespace XRouter.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal ConfigurationManager ConfigManager { get; set; }

        private ConfigurationTreeItem currentConfigurationTreeItem;

        private IConfigurationControl currentConfigurationControl;

        private DispatcherTimer xrouterStatusChecking;

        public MainWindow()
        {
            ConfigManager = new ConfigurationManager();

            Configurator.CustomItemTypes.Add(new TokenSelectionConfigurationItemType(() => new TokenSelectionEditor()));
            Configurator.CustomItemTypes.Add(new XrmUriConfigurationItemType(() => new XrmUriEditor(ConfigManager)));
            Configurator.CustomItemTypes.Add(new UriConfigurationItemType(() => new UriEditor()));

            InitializeComponent();

            #region Initialize tokens and logs viewers
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
            #endregion

            #region Initialize XrmEditor
            uiXrmEditor.Initialize(new XDocumentTypeDescriptor[] {
                new Xrm.DocumentTypeDescriptors.GeneralXDocumentTypeDescriptor(),
                new Xrm.DocumentTypeDescriptors.SchematronDocumentTypeDescriptor(),
                new Xrm.DocumentTypeDescriptors.XsltDocumentTypeDescriptor(),
            });
            #endregion

            #region XRouter status checking
            xrouterStatusChecking = new DispatcherTimer(TimeSpan.FromSeconds(10), DispatcherPriority.Normal, delegate {
                UpdateXRouterStatus();
            }, Dispatcher);
            xrouterStatusChecking.Start();
            #endregion
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Connect()) {
                Close();
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            xrouterStatusChecking.Stop();
        }

        private TreeViewItem CreateUIItem(ConfigurationTreeItem item)
        {
            TreeViewItem uiItem = new TreeViewItem
            {
                Tag = item,
                Header = item.Name
            };
            foreach (var child in item.Children)
            {
                TreeViewItem uiChild = CreateUIItem(child);
                uiItem.Items.Add(uiChild);
            }
            return uiItem;
        }

        private void uiConfigurationTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            uiConfigurationContainer.Content = null;
            currentConfigurationControl = null;
            if (e.NewValue == null)
            {
                return;
            }

            currentConfigurationTreeItem = (ConfigurationTreeItem)((TreeViewItem)e.NewValue).Tag;
            currentConfigurationControl = currentConfigurationTreeItem.ConfigurationControl;
            uiConfigurationContainer.Content = currentConfigurationControl;
        }

        private void uiConnect_Click(object sender, RoutedEventArgs e)
        {
            Connect();
        }

        private void uiNewConfig_Click(object sender, RoutedEventArgs e)
        {
            ConfigManager.ClearConfiguration();
            UpdateUI();
            uiStatusText.Text = "New configuration created.";
        }

        private void LoadFromServer_Click(object sender, RoutedEventArgs e)
        {
            if (ConfigManager.DownloadConfiguration(false)) {
                UpdateUI();
                uiStatusText.Text = "Configuration downloaded from server.";
            } else {
                Connect();
            }
        }

        private void SaveToServer_Click(object sender, RoutedEventArgs e)
        {
            #region This fix removes focus from TextBox in order to save its value
            TextBox uiFocusedTextBox = Keyboard.FocusedElement as TextBox;
            if (uiFocusedTextBox != null) {
                Panel uiContainer = uiFocusedTextBox.Parent as Panel;
                if (uiContainer != null) {
                    TextBox uiTempTextBox = new TextBox();
                    uiContainer.Children.Add(uiTempTextBox);
                    Keyboard.Focus(uiTempTextBox);
                    uiContainer.Children.Remove(uiTempTextBox);
                }
            }
            #endregion

            if (SaveConfiguration()) {
                ConfigManager.UploadConfiguration(false);
                uiStatusText.Text = "Configuration uploaded to server.";
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "XRouter configuration|*.xml";
            dialog.DefaultExt = ".xml";
            dialog.FileName = "config.xml";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == true)
            {
                XDocument xdoc = XDocument.Load(dialog.FileName);
                if (xdoc.Root.Name.LocalName != "configuration")
                {
                    MessageBox.Show("The file to be imported is not an XRouter configuration!",
                        "Import configuration");
                    return;
                }
                ConfigManager.ReadConfiguration(xdoc, false);
                UpdateUI();
                uiStatusText.Text = "Configuration imported.";
            }
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "XRouter configuration|*.xml";
            dialog.DefaultExt = ".xml";
            dialog.FileName = "config.xml";
            dialog.OverwritePrompt = true;
            if (dialog.ShowDialog() == true)
            {
                if (SaveConfiguration()) {
                    ConfigManager.Configuration.Content.XDocument.Save(dialog.FileName);
                    uiStatusText.Text = "Configuration exported.";
                }
            }
        }

        private void UpdateUI()
        {
            uiTokens.UpdateTokens();
            uiTraceLog.UpdateLogs();
            uiEventLog.UpdateLogs();

            #region Update configuration tree
            uiConfigurationTree.Items.Clear();
            ConfigurationTreeItem root = ConfigManager.CreateConfigurationTreeRoot();
            foreach (var item in root.Children) {
                TreeViewItem uiItem = CreateUIItem(item);
                uiConfigurationTree.Items.Add(uiItem);
            }
            #endregion

            uiXrmEditor.LoadContent(ConfigManager.Configuration.GetXrmContent());
        }

        private bool SaveConfiguration()
        {
            XDocument XrmContent = uiXrmEditor.GetXrmContent();
            if (XrmContent == null) {
                uiXrmEditorTab.IsSelected = true;
                return false;
            }
            ConfigManager.Configuration.SaveXrmContent(XrmContent.Root);

            #region Save configuration tree items
            var items = uiConfigurationTree.Items.OfType<TreeViewItem>().Select(i => (ConfigurationTreeItem)i.Tag);
            foreach (ConfigurationTreeItem item in items) {
                item.SaveRecursively();
            }
            #endregion

            return true;
        }

        private bool Connect()
        {
            ConnectionDialog dialog = new ConnectionDialog(ConfigManager);
            dialog.Owner = this;
            if (dialog.ShowDialog() != true) {
                return false;
            }
            UpdateUI();
            UpdateXRouterStatus();
            uiStatusText.Text = "Connected to XRouter manager.";
            return true;
        }

        private void uiStartXRouter_Click(object sender, RoutedEventArgs e)
        {
            try {
                ConfigManager.ConsoleServer.StartXRouterService(10);
            } catch (Exception ex) {
                string message = string.Format("Cannot start XRouter: " + ex.Message);
                MessageBox.Show(message, "Starting XRouter failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            UpdateXRouterStatus();
        }

        private void uiStopXRouter_Click(object sender, RoutedEventArgs e)
        {
            try {
                ConfigManager.ConsoleServer.StopXRouterService(10);
            } catch (Exception ex) {
                string message = string.Format("Cannot stop XRouter: " + ex.Message);
                MessageBox.Show(message, "Stopping XRouter failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            UpdateXRouterStatus();
        }

        private void UpdateXRouterStatus()
        {
            uiXRouterStatusIconIsRunning.Visibility = Visibility.Collapsed;
            uiXRouterStatusIconIsStopped.Visibility = Visibility.Collapsed;
            uiXRouterStatusIconUnknown.Visibility = Visibility.Collapsed;
            uiStartXRouter.IsEnabled = true;
            uiStopXRouter.IsEnabled = true;
            try {
                string statusText = ConfigManager.ConsoleServer.GetXRouterServiceStatus();
                uiXRouterStatusText.Text = statusText;
                ServiceControllerStatus status = (ServiceControllerStatus)Enum.Parse(typeof(ServiceControllerStatus), statusText);
                if (status == ServiceControllerStatus.Running) {
                    uiXRouterStatusIconIsRunning.Visibility = Visibility.Visible;
                    uiStartXRouter.IsEnabled = false;
                } else {
                    uiXRouterStatusIconIsStopped.Visibility = Visibility.Visible;
                }
                if (status == ServiceControllerStatus.Stopped) {
                    uiStopXRouter.IsEnabled = false;
                }
            } catch (Exception ex) {
                uiXRouterStatusIconUnknown.Visibility = Visibility.Visible;
                uiXRouterStatusText.Text = "Failed to determine status";
                uiXRouterStatusText.ToolTip = ex.Message;
            }
        }
    }
}
