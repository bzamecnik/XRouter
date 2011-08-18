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

namespace XRouter.Gui.Xrm
{
    /// <summary>
    /// Interaction logic for XrmEditor.xaml
    /// </summary>
    public partial class XrmEditor : UserControl
    {
        private XDocument XContent { get; set; }

        public XrmEditor()
        {
            InitializeComponent();
        }

        public void LoadContent(XDocument xContent)
        {
            XContent = xContent;
        }

        public XDocument SaveContent()
        {
            return XContent;
        }
    }
}
