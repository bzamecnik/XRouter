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

        private IEnumerable<XDocumentTypeDescriptor> documentTypeDescriptors;

        public XrmEditor()
        {
            InitializeComponent();
        }

        internal void Initialize(IEnumerable<XDocumentTypeDescriptor> documentTypeDescriptors)
        {
            this.documentTypeDescriptors = documentTypeDescriptors;
        }

        public void LoadContent(XDocument xContent)
        {
            XContent = xContent;
            ReloadTree();
        }

        public XDocument SaveContent()
        {
            return XContent;
        }

        private void SetActiveItem(XElement xItem, MenuItem uiItem)
        {
        }

        private void ReloadTree()
        {
            uiTree.Items.Clear();
            foreach (XElement xGroup in XContent.Root.Elements(XName.Get("group"))) {
                TreeViewItem uiGroup = CreateUIGroup(xGroup);
                uiTree.Items.Add(uiGroup);
            }
            foreach (XElement xItem in XContent.Root.Elements(XName.Get("item"))) {
                TreeViewItem uiItem = CreateUIItem(xItem);
                uiTree.Items.Add(uiItem);
            }

            uiTree.ContextMenu = new ContextMenu();
            uiTree.ContextMenu.Items.Add(CreateAddMenuItem(XContent.Root, null));
        }

        private TreeViewItem CreateUIGroup(XElement xGroup)
        {
            TreeViewItem uiGroup = new TreeViewItem {
                Tag = xGroup,
                Header = xGroup.Attribute(XName.Get("name")).Value
            };

            #region Create context menu
            uiGroup.ContextMenu = new ContextMenu();
            uiGroup.ContextMenu.Items.Add(CreateAddMenuItem(xGroup, uiGroup));
            uiGroup.ContextMenu.Items.Add(CreateRenameMenuItem(xGroup, uiGroup));
            uiGroup.ContextMenu.Items.Add(CreateRemoveMenuItem(xGroup, uiGroup));
            #endregion

            foreach (XElement xChildGroup in xGroup.Elements(XName.Get("group"))) {
                TreeViewItem uiChildGroup = CreateUIGroup(xChildGroup);
                uiGroup.Items.Add(uiChildGroup);
            }
            foreach (XElement xChildItem in xGroup.Elements(XName.Get("item"))) {
                TreeViewItem uiChildItem = CreateUIItem(xChildItem);
                uiGroup.Items.Add(uiChildItem);
            }
            return uiGroup;
        }

        private TreeViewItem CreateUIItem(XElement xItem)
        {
            TreeViewItem uiItem = new TreeViewItem {
                Header = xItem.Attribute(XName.Get("name")).Value,
            };

            #region Create context menu
            uiItem.ContextMenu = new ContextMenu();
            uiItem.ContextMenu.Items.Add(CreateRenameMenuItem(xItem, uiItem));
            uiItem.ContextMenu.Items.Add(CreateRemoveMenuItem(xItem, uiItem));
            #endregion

            return uiItem;
        }

        private MenuItem CreateAddMenuItem(XElement xParent, TreeViewItem uiParent)
        {
            MenuItem uiAdd = new MenuItem {
                Header = "Add..."
            };

            foreach (var docTypeDescriptorIterator in documentTypeDescriptors) {
                var docTypeDescriptor = docTypeDescriptorIterator;
                MenuItem uiDocType = new MenuItem {
                    Header = docTypeDescriptor.DocumentTypeName
                };
                uiDocType.Click += delegate {
                    XElement xItem = new XElement(XName.Get("item"));
                    xItem.SetAttributeValue(XName.Get("name"), CreateNewUniqueName(xParent, docTypeDescriptor.DocumentTypeName));
                    xItem.SetAttributeValue(XName.Get("type"), docTypeDescriptor.DocumentTypeName);
                    xParent.Add(xItem);
                    TreeViewItem uiItem = CreateUIItem(xItem);
                    if (uiParent != null) {
                        uiParent.Items.Add(uiItem);
                    } else {
                        uiTree.Items.Add(uiItem);
                    }
                };
                uiAdd.Items.Add(uiDocType);
            }

            uiAdd.Items.Add(new Separator());
            MenuItem uiAddGroup = new MenuItem {
                Header = "Group"
            };
            uiAddGroup.Click += delegate {
                XElement xGroup = new XElement(XName.Get("group"));
                xGroup.SetAttributeValue(XName.Get("name"), CreateNewUniqueName(xParent, "group"));
                xParent.Add(xGroup);
                TreeViewItem uiGroup = CreateUIGroup(xGroup);
                if (uiParent != null) {
                    uiParent.Items.Add(uiGroup);
                } else {
                    uiTree.Items.Add(uiGroup);
                }
            };
            uiAdd.Items.Add(uiAddGroup);

            return uiAdd;
        }

        private MenuItem CreateRemoveMenuItem(XElement xNode, TreeViewItem uiNode)
        {
            MenuItem uiRemove = new MenuItem {
                Header = "Remove"
            };
            uiRemove.Click += delegate {
                var mbr = MessageBox.Show("Do you want to delete this node?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                if (mbr != MessageBoxResult.Yes) {
                    return;
                }
                if (uiNode.Parent is TreeViewItem) {
                    ((TreeViewItem)uiNode.Parent).Items.Remove(uiNode);
                } else {
                    uiTree.Items.Remove(uiNode);
                }
                SetActiveItem(null, null);
                xNode.Remove();
            };
            return uiRemove;
        }

        private MenuItem CreateRenameMenuItem(XElement xNode, TreeViewItem uiNode)
        {
            MenuItem uiRename = new MenuItem {
                Header = "Rename"
            };
            uiRename.Click += delegate {
                string oldName = xNode.Attribute(XName.Get("name")).Value;
                string newName = oldName;
                while (true) {
                    newName = Microsoft.VisualBasic.Interaction.InputBox("Enter new name", "Renaming", newName);
                    if ((newName == null) || (newName.Trim().Length == 0) || (newName == oldName)) {
                        return;
                    }
                    var siblingNames = xNode.Parent.Elements().Select(e => e.Attribute(XName.Get("name")).Value);
                    if (siblingNames.Contains(newName)) {
                        MessageBox.Show("Given name already exists.", "Renaming failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }
                    break;
                };
                xNode.SetAttributeValue(XName.Get("name"), newName);
                uiNode.Header = newName;
            };
            return uiRename;
        }

        private string CreateNewUniqueName(XElement xParent, string baseName)
        {
            int index = 1;
            var siblingNames = xParent.Elements().Select(s => s.Attribute(XName.Get("name")).Value);
            while (siblingNames.Contains(baseName + index.ToString())) {
                index++;
            }
            return baseName + index.ToString();
        }
    }
}
