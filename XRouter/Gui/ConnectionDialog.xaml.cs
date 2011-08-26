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
using System.Windows.Shapes;
using XRouter.Gui.Properties;

namespace XRouter.Gui
{
    /// <summary>
    /// Interaction logic for ConnectionDialog.xaml
    /// </summary>
    public partial class ConnectionDialog : Window
    {
        private const string DaemonNtExecutableName = "DaemonNT.exe";
        private const string DaemonNtArguments = "debug xroutermanager";

        private ConfigurationManager configManager;

        internal ConnectionDialog(ConfigurationManager configManager)
        {
            InitializeComponent();

            this.configManager = configManager;

            string defaultAddress = configManager.DefaultConsoleServerAddress;
            if (!Settings.Default.ManagerAddresses.Contains(defaultAddress)) {
                Settings.Default.ManagerAddresses.Add(defaultAddress);
                Settings.Default.Save();
            }

            foreach (string address in Settings.Default.ManagerAddresses) {
                uiAddress.Items.Add(address);
            }
            uiAddress.SelectedIndex = 0;
        }

        private void uiConnect_Click(object sender, RoutedEventArgs e)
        {
            string address = uiAddress.Text;
            if (!configManager.Connect(address, false)) {
                return;
            }
            if (!configManager.DownloadConfiguration(false)) {
                return;
            }

            if (Settings.Default.ManagerAddresses.Contains(address)) {
                Settings.Default.ManagerAddresses.Remove(address);
            }
            Settings.Default.ManagerAddresses.Insert(0, address);
            Settings.Default.Save();

            DialogResult = true;
            Close();
        }

        private void uiRunLocalManager_Click(object sender, RoutedEventArgs e)
        {
            RunLocalXRouterManager();
        }

        private static void RunLocalXRouterManager()
        {
            string binPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string daemonNTPath = System.IO.Path.Combine(binPath, DaemonNtExecutableName);
            var serverProcesss = new System.Diagnostics.ProcessStartInfo(daemonNTPath, DaemonNtArguments);
            serverProcesss.WorkingDirectory = System.IO.Path.GetDirectoryName(daemonNTPath);
            System.Diagnostics.Process.Start(serverProcesss);
        }

        private void uiAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                uiConnect_Click(null, null);
            }
        }
    }
}
