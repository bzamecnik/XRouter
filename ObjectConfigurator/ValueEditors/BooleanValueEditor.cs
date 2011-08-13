using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectConfigurator.ItemTypes;
using ObjectConfigurator.ValueValidators;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;

namespace ObjectConfigurator.ValueEditors
{
    class BooleanValueEditor : ValueEditor
    {
        private BasicItemType basicValueType;

        private CheckBox uiValue;

        public BooleanValueEditor(ItemType valueType, IEnumerable<ValueValidatorAttribute> validators, XElement serializedDefaultValue)
            : base(valueType, validators, serializedDefaultValue)
        {
            basicValueType = (BasicItemType)ValueType;

            uiValue = new CheckBox {
                Margin = new Thickness(3),
                IsChecked = (serializedDefaultValue.Value == true.ToString())
            };
            uiValue.Click += uiValue_Click;

            Representation = new Border { Child = uiValue };
        }

        private void uiValue_Click(object sender, RoutedEventArgs e)
        {
            RaiseValueChanged();
        }

        public override bool WriteToXElement(XElement target)
        {
            target.Value = uiValue.IsChecked.Value.ToString();
            return true;
        }

        public override void ReadFromXElement(XElement source)
        {
            if (source == null) {
                source = SerializedDefaultValue;
            }

            uiValue.IsChecked = (source.Value == true.ToString());
        }
    }
}
