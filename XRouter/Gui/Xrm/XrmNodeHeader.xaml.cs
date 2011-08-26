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
using XRouter.Common.Xrm;

namespace XRouter.Gui.Xrm
{
    /// <summary>
    /// Interaction logic for XrmItemHeader.xaml
    /// </summary>
    public partial class XrmNodeHeader : UserControl
    {
        private XElement xNode;

        public XrmNodeHeader(XElement xNode, ImageSource iconSource)
        {
            InitializeComponent();

            this.xNode = xNode;
            uiIcon.Source = iconSource;
            Update();
        }

        public void Update()
        {
            uiName.Text = xNode.Attribute(XrmUri.NameAttributeName).Value;
        }
    }
}
