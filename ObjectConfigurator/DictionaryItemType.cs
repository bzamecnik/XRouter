using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;

namespace ObjectConfigurator
{
    class DictionaryItemType : ItemType
    {
        public ItemType KeyType { get; private set; }
        public ItemType ValueType { get; private set; }

        public DictionaryItemType(Type clrType, ItemType keyType, ItemType valueType)
            : base(clrType)
        {
            KeyType = keyType;
            ValueType = valueType;
        }

        public override void WriteToXElement(XElement target, object dictionary)
        {
            var pairs = (System.Collections.IEnumerable)dictionary;
            PropertyInfo keyProperty = typeof(KeyValuePair<,>).GetProperty("Key");
            PropertyInfo valueProperty = typeof(KeyValuePair<,>).GetProperty("Value");

            foreach (object pair in pairs) {
                object key = keyProperty.GetValue(pair, null);
                object value = valueProperty.GetValue(pair, null);

                XElement xPair = new XElement(XName.Get("pair"));

                XElement xKey = new XElement(XName.Get("key"));
                KeyType.WriteToXElement(xKey, key);
                xPair.Add(xKey);

                XElement xValue = new XElement(XName.Get("value"));
                ValueType.WriteToXElement(xValue, value);
                xPair.Add(xValue);

                target.Add(xPair);
            }
        }

        public override object ReadFromXElement(XElement source)
        {
            Type clrType = GetClrType();
            object dictionary = Activator.CreateInstance(clrType, true);
            MethodInfo addMethod = typeof(IDictionary<,>).GetMethod("Add");

            var xPairs = source.Elements(XName.Get("pair"));
            foreach (XElement xPair in xPairs) {
                XElement xKey = xPair.Element(XName.Get("key"));
                XElement xValue = xPair.Element(XName.Get("value"));
                object key = KeyType.ReadFromXElement(xKey);
                object value = ValueType.ReadFromXElement(xValue);
                addMethod.Invoke(dictionary, new object[] { key, value });
            }
            return dictionary;
        }
    }
}
