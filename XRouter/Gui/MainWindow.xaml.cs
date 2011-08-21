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

namespace XRouter.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal ConfigurationManager ConfigManager { get; set; }

        private ConfigurationTreeItem currentConfigurationTreeItem;

        private const string DaemonNtExecutableName = "DaemonNT.exe";
        private const string DaemonNtArguments = "debug xroutermanager";

        private IConfigurationControl currentConfigurationControl;

        public MainWindow()
        {
            ConfigManager = new ConfigurationManager();

            Configurator.CustomItemTypes.Add(new TokenSelectionConfigurationItemType(() => new TokenSelectionEditor()));
            Configurator.CustomItemTypes.Add(new XrmUriConfigurationItemType(() => new XrmUriEditor(ConfigManager)));
            Configurator.CustomItemTypes.Add(new UriConfigurationItemType(() => new UriEditor()));

            #region Run XRouter Manager if it is not already available
            try
            {
                ConfigManager.Connect();
                // test the connection
                ConfigManager.ConsoleServer.GetXRouterServiceStatus();
            }
            catch (Exception)
            {
                RunXRouterManager();
                try
                {
                    ConfigManager.Connect();
                    // test the connection
                    ConfigManager.ConsoleServer.GetXRouterServiceStatus();
                }
                catch (Exception secondEx)
                {
                    MessageBox.Show(
                        string.Format(
@"It is not possible to connect to XRouter Manager server at:
{0}.
Details:
{1}",
                        ConfigManager.ConsoleServerUri, secondEx.Message),
                        "Connection to XRouter Manager server");
                }

            }
            #endregion

            InitializeComponent();
        }

        private static void RunXRouterManager()
        {
            string binPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string daemonNTPath = System.IO.Path.Combine(binPath, DaemonNtExecutableName);
            var serverProcesss = new System.Diagnostics.ProcessStartInfo(daemonNTPath, DaemonNtArguments);
            serverProcesss.WorkingDirectory = System.IO.Path.GetDirectoryName(daemonNTPath);
            System.Diagnostics.Process.Start(serverProcesss);
            System.Threading.Thread.Sleep(2000);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            uiXrmEditor.Initialize(new XDocumentTypeDescriptor[] {
                new Xrm.DocumentTypeDescriptors.GeneralXDocumentTypeDescriptor(),
                new Xrm.DocumentTypeDescriptors.SchematronDocumentTypeDescriptor(),
                new Xrm.DocumentTypeDescriptors.XsltDocumentTypeDescriptor(),
            });

            ConfigManager.LoadConfigurationFromServer();
            LoadConfiguration();

            #region Load tokens and log records
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

        private void LoadFromServer_Click(object sender, RoutedEventArgs e)
        {
            ConfigManager.LoadConfigurationFromServer();
            LoadConfiguration();
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

            SaveConfiguration();
            ConfigManager.SaveConfigurationToServer();
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
                ConfigManager.Configuration = new ApplicationConfiguration(xdoc);
                LoadConfiguration();
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
                SaveConfiguration();
                ConfigManager.Configuration.Content.XDocument.Save(dialog.FileName);
            }
        }

        private void LoadConfiguration()
        {
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

        private void SaveConfiguration()
        {
            #region Save configuration tree items
            var items = uiConfigurationTree.Items.OfType<TreeViewItem>().Select(i => (ConfigurationTreeItem)i.Tag);
            foreach (ConfigurationTreeItem item in items) {
                item.SaveRecursively();
            }
            #endregion

            ConfigManager.Configuration.SaveXrmContent(uiXrmEditor.SaveContent().Root);
        }
    }
}
