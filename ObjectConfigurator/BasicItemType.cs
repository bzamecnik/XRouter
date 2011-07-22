using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;

namespace ObjectConfigurator
{
    class BasicItemType : ItemType
    {
        internal static readonly Type[] BasicClrTypes = new[] { 
            typeof(SByte), typeof(Int16), typeof(Int32), typeof(Int64), 
            typeof(Byte), typeof(UInt16), typeof(UInt32), typeof(UInt64),
            typeof(Single), typeof(Double),
            typeof(Boolean), typeof(String)
        };

        public BasicItemType(Type clrType)
            : base(clrType)
        {
        }

        public override void WriteToXElement(XElement target, object value)
        {
            string content = ToString(value);
            target.SetValue(content);
        }

        public override object ReadFromXElement(XElement source)
        {
            object result = Parse(source.Value);
            return result;
        }

        public string ToString(object value)
        {
            Type clrType = GetClrType();
            string result;
            if (clrType == typeof(string)) {
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
            Type clrType = GetClrType();
            object result;
            if (clrType == typeof(string)) {
                result = str;
            } else if (clrType == typeof(bool)) {
                MethodInfo parseMethod = clrType.GetMethod("Parse", new Type[] { typeof(string) });
                result = parseMethod.Invoke(null, new object[] { str });
            } else {
                MethodInfo parseMethod = clrType.GetMethod("Parse", new Type[] { typeof(string), typeof(CultureInfo) });
                result = parseMethod.Invoke(null, new object[] { str, CultureInfo.InvariantCulture });
            }
            return result;
        }
    }
}
