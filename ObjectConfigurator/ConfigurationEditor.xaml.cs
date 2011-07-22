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
using ObjectConfigurator.ItemEditors;
using System.Xml.Linq;

namespace ObjectConfigurator
{
    public partial class ConfigurationEditor : UserControl
    {
        public ClassMetadata ClassMetadata { get; private set; }

        private List<ItemEditor> itemEditors;

        internal ConfigurationEditor(ClassMetadata classMetadata)
        {
            InitializeComponent();
            ClassMetadata = classMetadata;

            PrepareItemEditors();
        }

        public void LoadConfiguration(XDocument config)
        {
            foreach (var itemEditor in itemEditors) {
                string itemName = itemEditor.Metadata.Name;
                XElement xItem = config.Root.Elements().FirstOrDefault(e => e.Attribute(Configurator.XName_ItemNameAttribute).Value == itemName);
                itemEditor.ReadFromXElement(xItem);
            }
        }

        public XDocument SaveConfiguration()
        {
            XElement xConfig = new XElement(Configurator.XName_RootElement);
            foreach (var itemEditor in itemEditors) {
                XElement xItem = new XElement(Configurator.XName_ItemElement);
                xItem.SetAttributeValue(Configurator.XName_ItemNameAttribute, itemEditor.Metadata.Name);
                itemEditor.WriteToXElement(xItem);
                xConfig.Add(xItem);
            }

            XDocument result = new XDocument();
            result.Add(xConfig);
            return result;
        }

        private void PrepareItemEditors()
        {
            itemEditors = new List<ItemEditor>();
            foreach (ItemMetadata itemMetadata in ClassMetadata.ConfigurableItems) {
                uiItemsContainer.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

                FrameworkElement header = CreateHeaderCell(itemMetadata);
                Grid.SetRow(header, uiItemsContainer.RowDefinitions.Count - 1);
                Grid.SetColumn(header, 0);
                header.VerticalAlignment = VerticalAlignment.Center;
                header.HorizontalAlignment = HorizontalAlignment.Right;
                header.Margin = new Thickness(5, 2, 5, 2);
                uiItemsContainer.Children.Add(header);

                ItemEditor itemEditor = ItemEditor.CreateEditor(itemMetadata);
                itemEditors.Add(itemEditor);

                Grid.SetRow(itemEditor.Representation, uiItemsContainer.RowDefinitions.Count - 1);
                Grid.SetColumn(itemEditor.Representation, 1);
                itemEditor.Representation.VerticalAlignment = VerticalAlignment.Center;
                itemEditor.Representation.Margin = new Thickness(5, 2, 5, 2);
                itemEditor.Representation.ToolTip = itemMetadata.UserDescription;
                uiItemsContainer.Children.Add(itemEditor.Representation);
            }
        }

        private FrameworkElement CreateHeaderCell(ItemMetadata itemMetadata)
        {
            Label result = new Label();
            result.Content = itemMetadata.UserName;
            result.ToolTip = itemMetadata.UserDescription;
            return result;
        }
    }
}
