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
using System.Security;

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

        private ConfigurationManager configManager;

        internal XrmUriEditor(ConfigurationManager configManager)
        {
            InitializeComponent();

            this.configManager = configManager;
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

        private void Choose_Click(object sender, RoutedEventArgs e)
        {
            #region Reload choose tree items
            uiChooseTree.Items.Clear();
            XDocument xrmContent = configManager.Configuration.GetXrmContent();
            foreach (XElement xNode in xrmContent.Root.Elements()) {
                TreeViewItem uiNode = CreateChooseTreeNode(xNode);
                uiChooseTree.Items.Add(uiNode);
            }
            #endregion
            uiChoosePopup.IsOpen = true;
        }

        private void ChooseSelect_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem uiSelectedItem = uiChooseTree.SelectedItem as TreeViewItem;
            if (uiSelectedItem == null) {
                return;
            }

            XElement xSelectedNode = (XElement)uiSelectedItem.Tag;
            if (xSelectedNode.Name != XrmUri.ItemElementName) {
                return;
            }

            string xpath = GetXrmNodeXPath(xSelectedNode);
            uiXPath.Text = xpath;
            ChangeXPath(xpath);
            uiChoosePopup.IsOpen = false;
        }

        private void ChooseCancel_Click(object sender, RoutedEventArgs e)
        {
            uiChoosePopup.IsOpen = false;
        }

        private TreeViewItem CreateChooseTreeNode(XElement xNode, TreeViewItem uiParent = null)
        {
            TreeViewItem uiNode = new TreeViewItem() {
                Tag = xNode,
                Header = xNode.Attribute(XrmUri.NameAttributeName).Value,
                IsExpanded = true
            };
            if (xNode.Name == XrmUri.GroupElementName) {
                uiNode.Foreground = Brushes.Gray;
                foreach (XElement xChild in xNode.Elements()) {
                    TreeViewItem uiChild = CreateChooseTreeNode(xChild, uiNode);
                    uiNode.Items.Add(uiChild);
                }
            }
            return uiNode;
        }

        private string GetXrmNodeXPath(XElement xNode)
        {
            if ((xNode.Name != XrmUri.ItemElementName) && (xNode.Name != XrmUri.GroupElementName)) {
                return string.Empty;
            }
            string parentXPath = GetXrmNodeXPath(xNode.Parent);
            string nodeName = xNode.Attribute(XrmUri.NameAttributeName).Value;
            string elementName = (xNode.Name == XrmUri.ItemElementName) ? XrmUri.ItemElementName.ToString() : XrmUri.GroupElementName.ToString();
            string xpath = parentXPath + string.Format("/{0}[@{1}='{2}']", elementName, XrmUri.NameAttributeName.LocalName, SecurityElement.Escape(nodeName));
            return xpath;
        }
    }
}
