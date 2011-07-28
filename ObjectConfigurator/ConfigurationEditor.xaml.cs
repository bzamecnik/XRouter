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
using ObjectConfigurator.ValueEditors;
using System.Xml.Linq;

namespace ObjectConfigurator
{
    public partial class ConfigurationEditor : UserControl
    {
        public ClassMetadata ClassMetadata { get; private set; }

        public event Action ConfigurationChanged = delegate { };

        private List<ValueEditor> valueEditors;

        internal ConfigurationEditor(ClassMetadata classMetadata)
        {
            InitializeComponent();
            ClassMetadata = classMetadata;

            PrepareItemEditors();
        }

        public void LoadConfiguration(XDocument config)
        {
            for (int i = 0; i < valueEditors.Count; i++) {
                ValueEditor valueEditor = valueEditors[i];
                string itemName = ClassMetadata.ConfigurableItems[i].Name;
                XElement xItem = config.Root.Elements().FirstOrDefault(e => e.Attribute(Configurator.XName_ItemNameAttribute).Value == itemName);
                valueEditor.ReadFromXElement(xItem);
            }
        }

        public XDocument SaveConfiguration()
        {
            XElement xConfig = new XElement(Configurator.XName_RootElement);
            for (int i = 0; i < valueEditors.Count; i++) {
                ValueEditor valueEditor = valueEditors[i];
                string itemName = ClassMetadata.ConfigurableItems[i].Name;
                XElement xItem = new XElement(Configurator.XName_ItemElement);
                xItem.SetAttributeValue(Configurator.XName_ItemNameAttribute, itemName);
                bool isValid = valueEditor.WriteToXElement(xItem);
                if (isValid) {
                    xConfig.Add(xItem);
                }
            }

            XDocument result = new XDocument();
            result.Add(xConfig);
            return result;
        }

        private void PrepareItemEditors()
        {
            valueEditors = new List<ValueEditor>();
            foreach (ItemMetadata itemMetadata in ClassMetadata.ConfigurableItems) {
                uiItemsContainer.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

                Border rowBackground = new Border {
                    Background = new LinearGradientBrush {
                        GradientStops = {
                            new GradientStop(Colors.White, -1),
                            new GradientStop(Colors.AliceBlue, 2)
                        }
                    },
                    Margin = new Thickness(2),
                    BorderBrush = Brushes.Gainsboro,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(5)
                };
                Grid.SetRow(rowBackground, uiItemsContainer.RowDefinitions.Count - 1);
                Grid.SetColumnSpan(rowBackground, 2);
                uiItemsContainer.Children.Add(rowBackground);

                FrameworkElement header = CreateHeaderCell(itemMetadata);
                Grid.SetRow(header, uiItemsContainer.RowDefinitions.Count - 1);
                Grid.SetColumn(header, 0);
                uiItemsContainer.Children.Add(header);

                ValueEditor itemEditor = ValueEditor.CreateEditor(itemMetadata.Type, itemMetadata.Validators, itemMetadata.SerializedDefaultValue);
                itemEditor.ValueChanged += delegate { ConfigurationChanged(); };
                valueEditors.Add(itemEditor);

                Grid.SetRow(itemEditor.Representation, uiItemsContainer.RowDefinitions.Count - 1);
                Grid.SetColumn(itemEditor.Representation, 1);
                itemEditor.Representation.VerticalAlignment = VerticalAlignment.Center;
                itemEditor.Representation.Margin = new Thickness(5, 6, 5, 8);
                itemEditor.Representation.ToolTip = itemMetadata.UserDescription;
                uiItemsContainer.Children.Add(itemEditor.Representation);
            }
        }

        private FrameworkElement CreateHeaderCell(ItemMetadata itemMetadata)
        {
            TextBlock result = new TextBlock();
            result.Margin = new Thickness(10, 8, 5, 0);
            result.VerticalAlignment = VerticalAlignment.Top;
            result.HorizontalAlignment = HorizontalAlignment.Right;
            result.FontWeight = FontWeights.Bold;
            result.Text = itemMetadata.UserName;
            result.ToolTip = itemMetadata.UserDescription;
            return result;
        }
    }
}
