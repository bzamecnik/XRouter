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
using ObjectConfigurator;
using System.Xml.Linq;

namespace XRouter.Gui.CommonControls
{
    /// <summary>
    /// Interaction logic for UriEditor.xaml
    /// </summary>
    public partial class UriEditor : UserControl, ICustomConfigurationValueEditor
    {
        public event Action ValueChanged = delegate { };

        FrameworkElement ICustomConfigurationValueEditor.Representation { get { return this; } }

        private Uri _uri = new Uri("about:blank");
        public Uri Uri {
            get { return _uri; }
            set {
                _uri = value;
                uiUri.Text = _uri.ToString();
                CheckValue();
            }
        }

        public UriEditor()
        {
            InitializeComponent();
        }

        private void uiUri_LostFocus(object sender, RoutedEventArgs e)
        {
            ChangeValue(uiUri.Text);
        }

        private void uiUri_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                ChangeValue(uiUri.Text);
            }
            if (e.Key == Key.Escape) {
                uiUri.Text = Uri.ToString();
            }
        }

        private void uiUri_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValue();
        }

        private void ChangeValue(string uriString)
        {
            Uri value;
            try {
                value = new Uri(uriString);
            } catch {
                return;
            }
            Uri = value;
            ValueChanged();
        }

        bool ICustomConfigurationValueEditor.WriteToXElement(XElement target)
        {
            target.Value = Uri.ToString();
            return true;
        }

        void ICustomConfigurationValueEditor.ReadFromXElement(XElement source)
        {
            Uri value;
            try {
                value = new Uri(source.Value);
            } catch {
                return;
            }
            Uri = value;
            uiUri.Text = value.ToString();
            CheckValue();
        }

        private void CheckValue()
        {
            bool isValid;
            try {
                Uri value = new Uri(uiUri.Text);
                isValid = true;
            } catch {
                isValid = false;
            }

            if (isValid) {
                uiUri.Background = Brushes.White;
            } else {
                uiUri.Background = Brushes.LightSalmon;
            }
        }
    }
}
