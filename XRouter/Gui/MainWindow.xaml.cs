﻿using System;
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
                var serverProcesss = new System.Diagnostics.ProcessStartInfo(daemonNTPath, "debug xrouter");
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
				// Neexistuje plugin pro editaci konfigurace				
				panelConfiguration.Children.Add(new Label() { Content = "Nebyl nalezen odpovídající plugin pro editaci" });
			}
			else
			{
				currentConfigurationControl = treeViewItem.Control;
				panelConfiguration.Children.Add(currentConfigurationControl as UserControl);
				// Tady se preda aktualni konfigurace v XML
                currentConfigurationControl.Initialize(ConfigurationManager.ApplicationConfiguration, ConfigurationManager.BrokerService, treeViewItem);
			}
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
            // TODO: fix a NullReferenceException
			currentConfigurationControl.Save();
            ConfigurationManager.BrokerService.ChangeConfiguration(ConfigurationManager.ApplicationConfiguration);
		}



		private void Clear_Click(object sender, RoutedEventArgs e)
		{
			currentConfigurationControl.Clear();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Configuration = ConfigurationManager.LoadConfigurationTree();
			this.DataContext = Configuration;
		}
	}
}