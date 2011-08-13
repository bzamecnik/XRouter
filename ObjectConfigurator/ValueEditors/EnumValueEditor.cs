using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Xml.Linq;
using ObjectConfigurator.ValueValidators;
using ObjectConfigurator.ItemTypes;

namespace ObjectConfigurator.ValueEditors
{
    class EnumValueEditor : ValueEditor
    {
        private ComboBox uiValue;
        private EnumItemType enumValueType;

        public EnumValueEditor(ItemType valueType, IEnumerable<ValueValidatorAttribute> validators, XElement serializedDefaultValue)
            : base(valueType, validators, serializedDefaultValue)
        {
            enumValueType = (EnumItemType)ValueType;

            uiValue = new ComboBox {
                IsEditable = false,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            uiValue.SelectionChanged += uiValue_SelectionChanged;

            Representation = uiValue;

            foreach (string valueName in enumValueType.ValueNames) {
                ComboBoxItem item = new ComboBoxItem {
                    Content = valueName,
                    Tag = valueName
                };
                uiValue.Items.Add(item);
            }
            uiValue.SelectedIndex = 0;
        }

        private void uiValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RaiseValueChanged();
        }

        public override bool WriteToXElement(XElement target)
        {
            string selectedValueName = (string)(((ComboBoxItem)uiValue.SelectedItem).Tag);
            target.Value = selectedValueName;
            return true;
        }

        public override void ReadFromXElement(XElement source)
        {
            if (source == null) {
                source = SerializedDefaultValue;
            }

            string valueName = source.Value;
            var uiItems = uiValue.Items.OfType<ComboBoxItem>();
            var uiItem = uiItems.FirstOrDefault(i => i.Tag.Equals(valueName));
            if (uiItem != null) {
                uiValue.SelectedItem = uiItem;
            }
        }
    }
}
