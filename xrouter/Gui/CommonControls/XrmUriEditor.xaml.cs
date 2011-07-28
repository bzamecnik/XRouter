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
using ObjectConfigurator;
using System.Xml.Linq;

namespace XRouter.Gui.CommonControls
{
    /// <summary>
    /// Interaction logic for XrmUriEditor.xaml
    /// </summary>
    public partial class XrmUriEditor : UserControl, ICustomConfigurationValueEditor
    {
        public event Action ValueChanged = delegate { };

        FrameworkElement ICustomConfigurationValueEditor.Representation { get { return this; } }

        private XrmUri _xrmUri;
        public XrmUri XrmUri {
            get { return _xrmUri; }
            set {
                _xrmUri = value;
                uiXPath.Text = _xrmUri.XPath;
                CheckValue();
            }
        }

        public XrmUriEditor()
        {
            InitializeComponent();

            XrmUri = new XrmUri();
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
            if (XrmUri.IsXPathValid(xpath)) {
                XrmUri.XPath = xpath;
                ValueChanged();
            }
        }

        bool ICustomConfigurationValueEditor.WriteToXElement(XElement target)
        {
            target.Value = XrmUri.XPath;
            return true;
        }

        void ICustomConfigurationValueEditor.ReadFromXElement(XElement source)
        {
            XrmUri.XPath = source.Value;
            uiXPath.Text = XrmUri.XPath;
            CheckValue();
        }

        private void uiXPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValue();
        }

        private void CheckValue()
        {
            bool isValid = XrmUri.IsXPathValid(uiXPath.Text);
            if (isValid) {
                uiXPath.Background = Brushes.White;
            } else {
                uiXPath.Background = Brushes.LightSalmon;
            }
        }
    }
}
