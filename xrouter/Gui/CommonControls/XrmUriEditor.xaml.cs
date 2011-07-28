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
using XRouter.Common.Xrm;

namespace XRouter.Gui.CommonControls
{
    /// <summary>
    /// Interaction logic for XrmUriEditor.xaml
    /// </summary>
    public partial class XrmUriEditor : UserControl
    {
        private XrmUri _xrmUri;
        public XrmUri XrmUri
        {
            get { return _xrmUri; }
            set {
                _xrmUri = value;
                uiXPath.Text = _xrmUri.XPath;
            }
        }

        public XrmUriEditor()
        {
            InitializeComponent();
        }

        private void uiXPath_LostFocus(object sender, RoutedEventArgs e)
        {
            ChangeXPath(uiXPath.Text);
        }

        private void uiXPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                ChangeXPath(uiXPath.Text);
            }
            if (e.Key == Key.Escape) {
                uiXPath.Text = XrmUri.XPath;
            }
        }

        private void ChangeXPath(string xpath)
        {
            XrmUri.XPath = xpath;
        }
    }
}
