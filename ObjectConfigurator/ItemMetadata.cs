using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;
using System.Globalization;
using System.Collections.ObjectModel;
using ObjectConfigurator.ValueValidators;

namespace ObjectConfigurator
{
    public enum ItemMemberKind
    {
        Field,
        Property
    }

    public class ItemMetadata
    {
        public ClassMetadata Owner { get; private set; }

        public ItemType Type { get; private set; }

        public string Name { get; private set; }

        public string UserName { get; private set; }

        public string UserDescription { get; private set; }

        public object DefaultValue { get; private set; }

        public ReadOnlyCollection<ValueValidatorAttribute> Validators { get; private set; }

        public ItemMemberKind MemberKind { get; private set; }

        public ItemMetadata(ClassMetadata owner, FieldInfo field, ConfigurationItemAttribute attribute, IEnumerable<ValueValidatorAttribute> validators)
            : this(owner, ItemMemberKind.Field, field.FieldType, field.Name, attribute, validators)
        {
        }

        public ItemMetadata(ClassMetadata owner, PropertyInfo property, ConfigurationItemAttribute attribute, IEnumerable<ValueValidatorAttribute> validators)
            : this(owner, ItemMemberKind.Property, property.PropertyType, property.Name, attribute, validators)
        {
        }

        private ItemMetadata(ClassMetadata owner, ItemMemberKind memberKind, Type clrType, string name, ConfigurationItemAttribute attribute, IEnumerable<ValueValidatorAttribute> validators)
        {
            Owner = owner;
            MemberKind = memberKind;
            Name = name;
            Type = ItemType.GetItemType(clrType);
            UserDescription = attribute.UserDescription;
            Validators = new ReadOnlyCollection<ValueValidatorAttribute>(validators.ToArray());

            if (attribute.UserName != null) {
                UserName = attribute.UserName;
            } else {
                UserName = Name;
            }

            VerifyDefaultValue(clrType, attribute.DefaultValue, Name);
            DefaultValue = attribute.DefaultValue;
        }

        private void VerifyDefaultValue(Type expectedType, object defaultValue, string itemName)
        {
            if (defaultValue != null) {
                if (!expectedType.IsAssignableFrom(defaultValue.GetType())) {
                    throw new InvalidOperationException(string.Format("Default value is not assignable to field/property \"{0}\"", itemName));
                }
            } else {
                if (!expectedType.IsClass) {
                    throw new InvalidOperationException(string.Format("Default value for field/property \"{0}\" is null but the field/property type is not class.", itemName));
                }
            }

            string errorDescription = string.Empty;
            bool isValid = Validators.All(v => v.IsValid(defaultValue, out errorDescription));
            if (!isValid) {
                throw new InvalidOperationException(string.Format("Default value for field/property \"{0}\" does not pass one or more validators. {1}", itemName, errorDescription));
            }
        }

        public object GetValue(object target)
        {
            switch (MemberKind) {
                case ItemMemberKind.Field:
                    FieldInfo field = Owner.GetClrType().GetField(Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    return field.GetValue(target);
                case ItemMemberKind.Property:
                    PropertyInfo property = Owner.GetClrType().GetProperty(Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    return property.GetValue(target, null);
                default:
                    throw new InvalidOperationException("Unknown member kind.");
            }
        }

        public void SetValue(object target, object value)
        {
            string errorDescription = string.Empty;
            bool isValid = Validators.All(v => v.IsValid(value, out errorDescription));
            if (!isValid) {
                throw new ArgumentException(string.Format("Value does not pass one or more validators. {0}", errorDescription), "value");
            }

            switch (MemberKind) {
                case ItemMemberKind.Field:
                    FieldInfo field = Owner.GetClrType().GetField(Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    field.SetValue(target, value);
                    break;
                case ItemMemberKind.Property:
                    PropertyInfo property = Owner.GetClrType().GetProperty(Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    property.SetValue(target, value, null);
                    break;
                default:
                    throw new InvalidOperationException("Unknown member kind.");
            }
        }

        public void WriteToXElement(XElement target, object value)
        {
            Type.WriteToXElement(target, value);
        }

        public object ReadFromXElement(XElement source)
        {
            return Type.ReadFromXElement(source);
        }
    }
}
