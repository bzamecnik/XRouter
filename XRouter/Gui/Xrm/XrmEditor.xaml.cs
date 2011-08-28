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
using XRouter.Gui.Utils;
using XRouter.Common.Xrm;

namespace XRouter.Gui.Xrm
{
    /// <summary>
    /// Interaction logic for XrmEditor.xaml
    /// </summary>
    public partial class XrmEditor : UserControl
    {
        private XDocument XContent { get; set; }

        private IEnumerable<XDocumentTypeDescriptor> documentTypeDescriptors;

        private XElement xActiveItem;
        private TreeViewItem uiActiveItem;

        public XrmEditor()
        {
            InitializeComponent();

            uiEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("XML");
            uiEditor.Options.ConvertTabsToSpaces = true;
            uiEditor.Options.IndentationSize = 4;
            uiEditor.ShowLineNumbers = true;
            uiEditorContainer.Visibility = Visibility.Collapsed;
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

        public XDocument GetXrmContent()
        {
            foreach (TreeViewItem uiNode in uiTree.Items) {
                if (!SaveItemContentRecursively(uiNode)) {
                    MessageBox.Show("Cannot save XRM content when it contains errors.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }

            return XContent;
        }

        private bool SaveItemContentRecursively(TreeViewItem uiNode)
        {
            XrmNodeHeader uiItemHeader = (XrmNodeHeader)uiNode.Tag;

            if (uiItemHeader.IsGroup) {
                foreach (TreeViewItem uiChild in uiNode.Items) {
                    if (!SaveItemContentRecursively(uiChild)) {
                        return false;
                    }
                }
                return true;

            } else {

                if (uiItemHeader.IsModified) {
                    string content = uiItemHeader.Content;
                    XDocument xDoc = Validate(content, uiItemHeader.XNode);
                    if (xDoc == null) {
                        ThreadUtils.InvokeLater(TimeSpan.FromSeconds(0.2), delegate {
                            uiNode.IsSelected = true;
                            Validate(content, uiItemHeader.XNode);
                        });
                        return false;
                    }
                    xActiveItem.Elements().First().Remove(); // remove old root of document
                    xActiveItem.Add(xDoc.Root); // add new root of document
                    uiItemHeader.IsModified = false;
                }
                return true;
            }
        }

        private void SetActiveItem(XElement xItem, TreeViewItem uiItem)
        {
            xActiveItem = xItem;
            uiActiveItem = uiItem;
            if (xItem == null) {
                uiEditorContainer.Visibility = Visibility.Collapsed;
                return;
            }

            XrmNodeHeader uiItemHeader = (XrmNodeHeader)uiItem.Tag;
            uiEditor.Text = uiItemHeader.Content;
            uiValidationStatus.Text = string.Empty;
            uiEditorContainer.Visibility = Visibility.Visible;
        }

        private void uiTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null) {
                SetActiveItem(null, null);
                return;
            }

            TreeViewItem uiSelectedItem = (TreeViewItem)e.NewValue;
            if (uiSelectedItem == uiActiveItem) {
                return;
            }

            XrmNodeHeader uiSelectedItemHeader = (XrmNodeHeader)uiSelectedItem.Tag;
            XElement xSelectedItem = uiSelectedItemHeader.XNode;
            if (xSelectedItem.Name != XrmUri.ItemElementName) {
                SetActiveItem(null, null);
                return;
            }

            SetActiveItem(xSelectedItem, uiSelectedItem);
        }

        private void uiEditor_TextChanged(object sender, EventArgs e)
        {
            XrmNodeHeader uiActiveItemHeader = (XrmNodeHeader)uiActiveItem.Tag;
            if (uiActiveItemHeader.Content != uiEditor.Text) {
                uiActiveItemHeader.Content = uiEditor.Text;
                uiActiveItemHeader.IsModified = uiActiveItemHeader.Content != uiActiveItemHeader.ContentFromXNode;
            }
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            Validate(uiEditor.Text, xActiveItem);
        }

        private XDocument Validate(string content, XElement xItem)
        {
            bool isValid;
            string errorDescription = null;

            XDocument xDocument = null;
            try {
                xDocument = XDocument.Parse(content);
                isValid = true;
            } catch (Exception ex) {
                errorDescription = "Xml is not well-formed: " + Environment.NewLine + ex.Message;
                isValid = false;
            }

            if (isValid) {
                string documentTypeName = xItem.Attribute(XrmUri.TypeAttributeName).Value;
                XDocumentTypeDescriptor docTypeDescriptor = documentTypeDescriptors.FirstOrDefault(d => d.DocumentTypeName == documentTypeName);
                if (docTypeDescriptor == null) {
                    errorDescription = "Unknown document type: " + documentTypeName;
                    isValid = false;
                } else {
                    isValid = docTypeDescriptor.IsValid(xDocument, out errorDescription);
                }
            }

            if (isValid) {
                uiValidationStatus.Text = "Document is valid.";
                uiValidationStatus.Foreground = Brushes.DarkGreen;
                return xDocument;
            } else {
                uiValidationStatus.Text = errorDescription ?? "Document is invalid.";
                uiValidationStatus.Foreground = Brushes.DarkRed;
                return null;
            }
        }

        private void ReloadTree()
        {
            uiTree.Items.Clear();
            foreach (XElement xGroup in XContent.Root.Elements(XrmUri.GroupElementName)) {
                TreeViewItem uiGroup = CreateUIGroup(xGroup);
                uiTree.Items.Add(uiGroup);
            }
            foreach (XElement xItem in XContent.Root.Elements(XrmUri.ItemElementName)) {
                TreeViewItem uiItem = CreateUIItem(xItem);
                uiTree.Items.Add(uiItem);
            }

            uiTree.ContextMenu = new ContextMenu();
            uiTree.ContextMenu.Items.Add(CreateAddMenuItem(XContent.Root, null));
        }

        private TreeViewItem CreateUIGroup(XElement xGroup)
        {
            XrmNodeHeader uiGroupHeader = new XrmNodeHeader(xGroup, new BitmapImage(new Uri("pack://application:,,,/XRouter.GUI;component/Resources/Actions-view-list-icons-icon.png")));
            TreeViewItem uiGroup = new TreeViewItem {
                Tag = uiGroupHeader,
                Header = uiGroupHeader
            };

            #region Create context menu
            uiGroup.ContextMenu = new ContextMenu();
            uiGroup.ContextMenu.Items.Add(CreateAddMenuItem(xGroup, uiGroup));
            uiGroup.ContextMenu.Items.Add(CreateRenameMenuItem(xGroup, uiGroup));
            uiGroup.ContextMenu.Items.Add(CreateRemoveMenuItem(xGroup, uiGroup));
            #endregion

            foreach (XElement xChildGroup in xGroup.Elements(XrmUri.GroupElementName)) {
                TreeViewItem uiChildGroup = CreateUIGroup(xChildGroup);
                uiGroup.Items.Add(uiChildGroup);
            }
            foreach (XElement xChildItem in xGroup.Elements(XrmUri.ItemElementName)) {
                TreeViewItem uiChildItem = CreateUIItem(xChildItem);
                uiGroup.Items.Add(uiChildItem);
            }
            return uiGroup;
        }

        private TreeViewItem CreateUIItem(XElement xItem)
        {
            string documentTypeName = xItem.Attribute(XrmUri.TypeAttributeName).Value;
            var docTypeDescriptor = documentTypeDescriptors.FirstOrDefault(d => d.DocumentTypeName == documentTypeName);

            XrmNodeHeader uiItemHeader = new XrmNodeHeader(xItem, docTypeDescriptor.GetIconSource());
            TreeViewItem uiItem = new TreeViewItem {
                Tag = uiItemHeader,
                Header = uiItemHeader
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
                    Header = docTypeDescriptor.DocumentTypeName,
                    Icon = new Image {
                        Height = 20,
                        Source = docTypeDescriptor.GetIconSource()
                    }
                };
                uiDocType.Click += delegate {
                    XElement xItem = new XElement(XrmUri.ItemElementName);
                    xItem.SetAttributeValue(XrmUri.NameAttributeName, CreateNewUniqueName(xParent, docTypeDescriptor.DocumentTypeName));
                    xItem.SetAttributeValue(XrmUri.TypeAttributeName, docTypeDescriptor.DocumentTypeName);
                    xItem.Add(docTypeDescriptor.CreateDefaultRoot());
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
                Header = "Group",
                Icon = new Image {
                    Height = 20,
                    Source = new BitmapImage(new Uri("pack://application:,,,/XRouter.GUI;component/Resources/Actions-view-list-icons-icon.png"))
                }
            };
            uiAddGroup.Click += delegate {
                XElement xGroup = new XElement(XrmUri.GroupElementName);
                xGroup.SetAttributeValue(XrmUri.NameAttributeName, CreateNewUniqueName(xParent, "group"));
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
                string oldName = xNode.Attribute(XrmUri.NameAttributeName).Value;
                string newName = oldName;
                while (true) {
                    newName = Microsoft.VisualBasic.Interaction.InputBox("Enter new name", "Renaming", newName);
                    if ((newName == null) || (newName.Trim().Length == 0) || (newName == oldName)) {
                        return;
                    }
                    var siblingNames = xNode.Parent.Elements().Select(e => e.Attribute(XrmUri.NameAttributeName).Value);
                    if (siblingNames.Contains(newName)) {
                        MessageBox.Show("Given name already exists.", "Renaming failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }
                    break;
                };
                xNode.SetAttributeValue(XrmUri.NameAttributeName, newName);
                ((XrmNodeHeader)uiNode.Header).Update();
            };
            return uiRename;
        }

        private string CreateNewUniqueName(XElement xParent, string baseName)
        {
            int index = 1;
            var siblingNames = xParent.Elements().Select(s => s.Attribute(XrmUri.NameAttributeName).Value);
            while (siblingNames.Contains(baseName + index.ToString())) {
                index++;
            }
            return baseName + index.ToString();
        }
    }
}
