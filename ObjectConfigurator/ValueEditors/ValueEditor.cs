using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using ObjectConfigurator.ValueValidators;
using ObjectConfigurator.ItemTypes;

namespace ObjectConfigurator.ValueEditors
{
    abstract class ValueEditor
    {
        public ItemType ValueType { get; private set; }

        public IEnumerable<ValueValidatorAttribute> Validators { get; private set; }

        protected XElement SerializedDefaultValue { get; private set; }

        public FrameworkElement Representation { get; protected set; }

        public event Action ValueChanged = delegate { };

        protected ValueEditor(ItemType valueType, IEnumerable<ValueValidatorAttribute> validators, XElement serializedDefaultValue)
        {
            ValueType = valueType;
            Validators = validators;
            SerializedDefaultValue = serializedDefaultValue;
        }

        public static ValueEditor CreateEditor(ItemType valueType, IEnumerable<ValueValidatorAttribute> validators, XElement serializedDefaultValue)
        {
            if (valueType is BasicItemType) {
                if (valueType.ClrTypeFullName == typeof(Boolean).FullName) {
                    return new BooleanValueEditor(valueType, validators, serializedDefaultValue);
                } else {
                    return new TextualBasicValueEditor(valueType, validators, serializedDefaultValue);
                }
            }
            if (valueType is EnumItemType) {
                return new EnumValueEditor(valueType, validators, serializedDefaultValue);
            }
            if (valueType is CollectionItemType) {
                return new CollectionValueEditor(valueType, validators, serializedDefaultValue);
            }
            if (valueType is DictionaryItemType) {
                return new DictionaryValueEditor(valueType, validators, serializedDefaultValue);
            }
            if (valueType is CustomItemType) {
                return new CustomValueEditor(valueType, validators, serializedDefaultValue);
            }
            throw new InvalidOperationException("Unknown item type.");
        }

        protected void RaiseValueChanged()
        {
            ValueChanged();
        }

        protected bool IsValid(object value, out string errorDescription)
        {
            foreach (var validator in Validators) {
                if (!validator.IsValid(value, out errorDescription)) {
                    return false;
                }
            }
            errorDescription = null;
            return true;
        }

        public abstract bool WriteToXElement(XElement target);

        public abstract void ReadFromXElement(XElement source);
    }
}
