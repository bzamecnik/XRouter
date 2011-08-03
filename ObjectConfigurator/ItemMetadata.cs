using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Linq;
using ObjectConfigurator.ItemTypes;
using ObjectConfigurator.ValueValidators;

namespace ObjectConfigurator
{
    [DataContract]
    public enum ItemMemberKind
    {
        [EnumMember]
        Field,
        [EnumMember]
        Property
    }

    [DataContract]
    [KnownType(typeof(BasicItemType))]
    [KnownType(typeof(EnumItemType))]
    [KnownType(typeof(CollectionItemType))]
    [KnownType(typeof(DictionaryItemType))]
    [KnownType(typeof(CustomItemType))]
    [KnownType(typeof(CountRangeValidatorAttribute))]
    [KnownType(typeof(RangeValidatorAttribute))]
    [KnownType(typeof(RegexValidatorAttribute))]
    public class ItemMetadata
    {
        [DataMember]
        public ClassMetadata Owner { get; private set; }

        [DataMember]
        public ItemType Type { get; private set; }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public string UserName { get; private set; }

        [DataMember]
        public string UserDescription { get; private set; }

        [DataMember]
        public XElement SerializedDefaultValue { get; private set; }

        [DataMember]
        public ReadOnlyCollection<ValueValidatorAttribute> Validators { get; private set; }

        [DataMember]
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
            SerializedDefaultValue = new XElement(XName.Get("defaultValue"));

            #region In case of dictionary type, convert default array of keys and values into dictionary
            if ((Type is DictionaryItemType) && (attribute.DefaultValue != null)) {
                Dictionary<object, object> defaultDictionary = new Dictionary<object, object>();
                Array defaultArray = (Array)attribute.DefaultValue;
                for (int i = 0; i < defaultArray.Length / 2; i++) {
                    object key = defaultArray.GetValue((i * 2) + 0);
                    object value = defaultArray.GetValue((i * 2) + 1);
                    defaultDictionary.Add(key, value);
                }
                attribute.DefaultValue = defaultDictionary;
            }
            #endregion

            if (Type is CustomItemType) {
                CustomItemType customType = (CustomItemType)Type;
                customType.GetCustomType().WriteDefaultValueToXElement(SerializedDefaultValue, attribute.DefaultValue);
            } else {
                Type.WriteToXElement(SerializedDefaultValue, attribute.DefaultValue);
            }
        }

        public object GetDefaultValue()
        {
            object result = Type.ReadFromXElement(SerializedDefaultValue);
            return result;
        }

        private void VerifyDefaultValue(Type expectedType, object defaultValue, string itemName)
        {
            if (Type is CustomItemType) {
                return;
            } else if (Type is CollectionItemType) {
                if (defaultValue != null) {
                    Type defaultValueType = defaultValue.GetType();
                    Type expectedElementType = ((CollectionItemType)Type).ElementType.GetClrType();
                    if ((!defaultValueType.IsArray) || (!expectedElementType.IsAssignableFrom(defaultValueType.GetElementType()))) {
                        throw new InvalidOperationException(string.Format("Default value of field/property \"{0}\" must be array which has the same type of elements as has this field/property.", itemName));
                    }
                }
            } else if (Type is DictionaryItemType) {
                if (defaultValue != null) {
                    Type defaultValueType = defaultValue.GetType();
                    if (!defaultValueType.IsArray) {
                        throw new InvalidOperationException(string.Format("Default value of field/property \"{0}\" must be array containing keys and values in pairs.", itemName));
                    }
                    Array defaultValueArray = (Array)defaultValue;
                    if (defaultValueArray.Length % 2 != 0) {
                        throw new InvalidOperationException(string.Format("Default value of field/property \"{0}\" must be array containing keys and values in pairs.", itemName));
                    }
                    Type expectedKeyType = ((DictionaryItemType)Type).KeyType.GetClrType();
                    Type expectedValueType = ((DictionaryItemType)Type).ValueType.GetClrType();
                    for (int i = 0; i < defaultValueArray.Length / 2; i++) {
                        #region Determine keyType and valueType
                        object key = defaultValueArray.GetValue((i * 2) + 0);
                        object value = defaultValueArray.GetValue((i * 2) + 1);
                        Type keyType;
                        Type valueType;
                        if (key != null) {
                            keyType = key.GetType();
                        } else {
                            keyType = typeof(string);
                        }
                        if (value != null) {
                            valueType = value.GetType();
                        } else {
                            valueType = typeof(string);
                        }
                        #endregion
                        if (!expectedKeyType.IsAssignableFrom(keyType)) {
                            throw new InvalidOperationException(string.Format("Default value of field/property \"{0}\" contains key at index {1} which has invalid type {2}. Expected key type is {3}.", itemName, i * 2, keyType, expectedKeyType));
                        }
                        if (!expectedValueType.IsAssignableFrom(valueType)) {
                            throw new InvalidOperationException(string.Format("Default value of field/property \"{0}\" contains value at index {1} which has invalid type {2}. Expected value type is {3}.", itemName, (i * 2) + 1, valueType, expectedValueType));
                        }
                    }
                }
            } else {
                if (defaultValue != null) {
                    if (!expectedType.IsAssignableFrom(defaultValue.GetType())) {
                        throw new InvalidOperationException(string.Format("Default value is not assignable to field/property \"{0}\"", itemName));
                    }
                } else {
                    if (!expectedType.IsClass) {
                        throw new InvalidOperationException(string.Format("Default value for field/property \"{0}\" is null but the field/property type is not class.", itemName));
                    }
                }
            }

            string errorDescription;
            bool isValid = IsValid(defaultValue, out errorDescription);
            if (!isValid) {
                throw new InvalidOperationException(string.Format("Default value for field/property \"{0}\" does not pass one or more validators. {1}", itemName, errorDescription));
            }
        }

        public bool IsValid(object value, out string errorDescription)
        {
            foreach (var validator in Validators) {
                if (!validator.IsValid(value, out errorDescription)) {
                    return false;
                }
            }
            errorDescription = null;
            return true;
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
