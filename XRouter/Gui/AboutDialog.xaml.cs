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
            try {
                Process.Start(ProjectUrl);
            } catch(Exception ex) {
                string messsage = string.Format("Cannot open homepage \"{0}\".{1}{1}{2}", ProjectUrl, Environment.NewLine, ex.Message);
                MessageBox.Show(messsage, "Open homepage", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
