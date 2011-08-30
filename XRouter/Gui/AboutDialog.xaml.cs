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
using System.Diagnostics;

namespace XRouter.Gui
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : Window
    {
        private static readonly string ProjectUrl = "http://www.assembla.com/spaces/xrouter";

        public AboutDialog()
        {
            InitializeComponent();
        }

        private void uiOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            OpenLocation(ProjectUrl, "Open homepage");
        }

        private void License_Click(object sender, RoutedEventArgs e)
        {
            string licenseFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Documentation\LICENSE.txt");
            OpenLocation(licenseFile, "Open license information");
        }


        private void Authors_Click(object sender, RoutedEventArgs e)
        {
            string authorsFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Documentation\AUTHORS.txt");
            OpenLocation(authorsFile, "Open information on authors");
        }

        private static void OpenLocation(string uri, string title)
        {
            try
            {
                Process.Start(uri);
            }
            catch (Exception ex)
            {
                string messsage = string.Format("Cannot open \"{0}\".{1}{1}{2}", uri, Environment.NewLine, ex.Message);
                MessageBox.Show(messsage, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
