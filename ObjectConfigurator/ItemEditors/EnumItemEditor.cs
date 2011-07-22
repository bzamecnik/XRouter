using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Xml.Linq;

namespace ObjectConfigurator.ItemEditors
{
    class EnumItemEditor : ItemEditor
    {
        private ComboBox uiValue;
        private EnumItemType enumItemType;

        public EnumItemEditor(ItemMetadata metadata)
            : base(metadata)
        {
            enumItemType = (EnumItemType)metadata.Type;

            uiValue = new ComboBox {
                IsEditable = false,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            Representation = uiValue;

            foreach (string valueName in enumItemType.ValueNames) {
                ComboBoxItem item = new ComboBoxItem {
                    Content = valueName,
                    Tag = valueName
                };
                uiValue.Items.Add(item);
            }
            uiValue.SelectedIndex = 0;
        }

        public override void WriteToXElement(XElement target)
        {
            string selectedValueName = (string)(((ComboBoxItem)uiValue.SelectedItem).Tag);
            target.Value = selectedValueName;
        }

        public override void ReadFromXElement(XElement source)
        {
            string valueName = source.Value;

            var uiItems = uiValue.Items.OfType<ComboBoxItem>();
            var uiItem = uiItems.FirstOrDefault(i => i.Tag.Equals(valueName));
            if (uiItem != null) {
                uiValue.SelectedItem = uiItem;
            }
        }
    }
}
