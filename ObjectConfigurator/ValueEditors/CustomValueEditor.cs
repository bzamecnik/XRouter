using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectConfigurator.ItemTypes;
using System.Windows.Controls;
using ObjectConfigurator.ValueValidators;
using System.Xml.Linq;

namespace ObjectConfigurator.ValueEditors
{
    class CustomValueEditor : ValueEditor
    {
        private ComboBox uiValue;
        private CustomItemType customValueType;
        private ICustomConfigurationItemType customType;
        private ICustomConfigurationValueEditor customEditor;

        public CustomValueEditor(ItemType valueType, IEnumerable<ValueValidatorAttribute> validators, XElement serializedDefaultValue)
            : base(valueType, validators, serializedDefaultValue)
        {
            customValueType = (CustomItemType)ValueType;
            customType = customValueType.GetCustomType();
            customEditor = customType.CreateEditor();

            Representation = customEditor.Representation;

            customEditor.ValueChanged += customEditor_ValueChanged;
        }

        private void customEditor_ValueChanged()
        {
            RaiseValueChanged();
        }

        public override bool WriteToXElement(XElement target)
        {
            return customEditor.WriteToXElement(target);
        }

        public override void ReadFromXElement(XElement source)
        {
            if (source == null) {
                source = new XElement(XName.Get("defaultValue"));
                customType.WriteDefaultValueToXElement(source);
            }
            customEditor.ReadFromXElement(source);
        }
    }
}
