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
using Microsoft.Win32;
using ObjectConfigurator;
using XRouter.Common;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Xrm;
using XRouter.Gui.CommonControls;

namespace XRouter.Gui
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public ConfigurationTree Configuration;

		IConfigurationControl currentConfigurationControl;

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

		private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			panelConfiguration.Children.Clear();
			currentConfigurationControl = null;
			ConfigurationTree treeViewItem = e.NewValue as ConfigurationTree;

			if (treeViewItem.Control == null)
			{
				panelConfiguration.Children.Add(new Label() { Content =
                    "There is no available plugin for editing this configuration item." });
			}
			else
			{
				currentConfigurationControl = treeViewItem.Control;
				panelConfiguration.Children.Add(currentConfigurationControl as UserControl);
				// Tady se preda aktualni konfigurace v XML
                currentConfigurationControl.Initialize(ConfigurationManager.ApplicationConfiguration,
                    ConfigurationManager.ConsoleServer, treeViewItem);
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
                ConfigurationManager.ApplicationConfiguration = new ApplicationConfiguration(xdoc);
                FillConfigurationToGui();
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
                if (currentConfigurationControl != null)
                {
                    currentConfigurationControl.Save();
                    ConfigurationManager.ApplicationConfiguration.Content.XDocument.Save(dialog.FileName);
                }
            }
		}

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (currentConfigurationControl != null)
            {
                currentConfigurationControl.Save();
                ConfigurationManager.ConsoleServer.ChangeConfiguration(ConfigurationManager.ApplicationConfiguration);
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.ApplicationConfiguration = ConfigurationManager.GetConfigurationFromServer();
            FillConfigurationToGui();
        }

		private void Clear_Click(object sender, RoutedEventArgs e)
		{
            if (currentConfigurationControl != null)
            {
                currentConfigurationControl.Clear();
            }
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
            ConfigurationManager.ApplicationConfiguration = ConfigurationManager.GetConfigurationFromServer();
            FillConfigurationToGui();
		}

        private void FillConfigurationToGui()
        {
            Configuration = ConfigurationManager.GetConfigurationTree(
                ConfigurationManager.ApplicationConfiguration);
            this.DataContext = Configuration;
        }
	}
}
