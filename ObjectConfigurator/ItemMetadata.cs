using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;
using System.Globalization;

namespace ObjectConfigurator
{
    class ItemMetadata
    {
        public Type Type { get; private set; }

        public string Name { get; private set; }

        public string UserName { get; private set; }

        public string UserDescription { get; private set; }

        private FieldInfo field;
        private PropertyInfo property;

        public ItemMetadata(FieldInfo field, ConfigurationItemAttribute attribute)
        {
            this.field = field;
            Name = field.Name;
            Type = field.FieldType;

            if (attribute.UserName != null) {
                UserName = attribute.UserName;
            } else {
                UserName = Name;
            }

            UserDescription = attribute.UserDescription;
        }

        public ItemMetadata(PropertyInfo property, ConfigurationItemAttribute attribute)
        {
            this.property = property;
            Name = property.Name;
            Type = property.PropertyType;

            if (attribute.UserName != null) {
                UserName = attribute.UserName;
            } else {
                UserName = Name;
            }

            UserDescription = attribute.UserDescription;
        }

        public object GetValue(object target)
        {
            if (field != null) {
                return field.GetValue(target);
            } else {
                return property.GetValue(target, null);
            }
        }

        public void SetValue(object target, object value)
        {
            if (field != null) {
                field.SetValue(target, value);
            } else {
                property.SetValue(target, value, null);
            }
        }

        public void WriteToXElement(XElement target, object value)
        {
            string content = ToString(value);
            target.SetValue(content);
        }

        public object ReadFromXElement(XElement source)
        {
            object result = Parse(source.Value);
            return result;
        }

        public string ToString(object value)
        {
            string result;
            if (Type == typeof(string)) {
                result = (string)value;
            } else if (value is IFormattable) {
                result = ((IFormattable)value).ToString(null, CultureInfo.InvariantCulture);
            } else {
                result = value.ToString();
            }
            return result;
        }

        public object Parse(string str)
        {
            object result;
            if (Type == typeof(string)) {
                result = str;
            } else if (typeof(Enum).IsAssignableFrom(Type)) {
                result = Enum.Parse(Type, str);
            } else if (Type == typeof(bool)) {
                MethodInfo parseMethod = Type.GetMethod("Parse", new Type[] { typeof(string) });
                result = parseMethod.Invoke(null, new object[] {str });
            } else {
                MethodInfo parseMethod = Type.GetMethod("Parse", new Type[] { typeof(string), typeof(CultureInfo) });
                result = parseMethod.Invoke(null, new object[] { str, CultureInfo.InvariantCulture });
            }
            return result;
        }
    }
}
