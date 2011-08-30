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
        public XElement XNode { get; private set; }

        public bool IsGroup {
            get { return XNode.Name == XrmUri.GroupElementName; }
        }

        private bool _isModified = false;
        public bool IsModified { 
            get { return _isModified; }
            set {
                _isModified = value;
                if (value) {
                    uiIsModified.Visibility = Visibility.Visible;
                } else {
                    uiIsModified.Visibility = Visibility.Collapsed;
                }
            }
        }

        public string Content { get; set; }

        public string ContentFromXNode {
            get { return XNode.Elements().First().ToString(); }
        }

        public XrmNodeHeader(XElement xNode, ImageSource iconSource)
        {
            InitializeComponent();

            XNode = xNode;

            if (!IsGroup) {
                XElement xRoot = xNode.Elements().FirstOrDefault();
                if (xRoot != null) {
                    Content = xRoot.ToString();
                } else {
                    Content = "<root></root>";
                }
            }

            uiIcon.Source = iconSource;
            Update();
        }

        public void Update()
        {
            uiName.Text = XNode.Attribute(XrmUri.NameAttributeName).Value;
        }
    }
}
