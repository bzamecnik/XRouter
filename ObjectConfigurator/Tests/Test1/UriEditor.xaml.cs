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

namespace ObjectConfigurator.Tests.Test1
{
    /// <summary>
    /// Interaction logic for UriEditor.xaml
    /// </summary>
    public partial class UriEditor : UserControl, ICustomValueEditor
    {
        public event Action ValueChanged = delegate { };

        public FrameworkElement Representation { get { return this; } }

        public UriEditor()
        {
            InitializeComponent();
        }

        private void uiValue_LostFocus(object sender, RoutedEventArgs e)
        {
            ValueChanged();
        }

        public bool WriteToXElement(XElement target)
        {
            try {
                Uri url = new Uri(uiValue.Text);
                target.Value = url.ToString();
                return true;
            } catch {
                return false;
            }
        }

        public void ReadFromXElement(XElement source)
        {
            uiValue.Text = source.Value;
        }
    }
}
