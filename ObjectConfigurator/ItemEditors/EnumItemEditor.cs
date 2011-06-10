using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace ObjectConfigurator.ItemEditors
{
    class EnumItemEditor : ItemEditor
    {
        private ComboBox uiValue;

        public EnumItemEditor(ItemMetadata metadata)
            : base(metadata)
        {
            uiValue = new ComboBox {
                IsEditable = false,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            Representation = uiValue;

            foreach (object value in Enum.GetValues(metadata.Type)) {
                ComboBoxItem item = new ComboBoxItem {
                    Content = value,
                    Tag = value
                };
                uiValue.Items.Add(item);
            }
            uiValue.SelectedIndex = 0;
        }

        public override void SetValue(object value)
        {
            var uiItems = uiValue.Items.OfType<ComboBoxItem>().ToArray();
            var uiItem = uiItems.FirstOrDefault(i => i.Tag.Equals(value));
            uiValue.SelectedItem = uiItem;
        }

        public override object GetValue()
        {
            object result = ((ComboBoxItem)uiValue.SelectedItem).Tag;
            return result;
        }
    }
}
