using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace ObjectConfigurator.ItemTypes
{
    [DataContract]
    [KnownType(typeof(BasicItemType))]
    [KnownType(typeof(EnumItemType))]
    [KnownType(typeof(CollectionItemType))]
    [KnownType(typeof(DictionaryItemType))]
    [KnownType(typeof(CustomItemType))]
    class DictionaryItemType : ItemType
    {
        internal static readonly XName XName_PairElement = XName.Get("pair");
        internal static readonly XName XName_KeyElement = XName.Get("key");
        internal static readonly XName XName_ValueElement = XName.Get("value");

        [DataMember]
        public ItemType KeyType { get; private set; }

        [DataMember]
        public ItemType ValueType { get; private set; }

        public DictionaryItemType(Type clrType, ItemType keyType, ItemType valueType)
            : base(clrType)
        {
            KeyType = keyType;
            ValueType = valueType;
        }

        public override void WriteToXElement(XElement target, object dictionary)
        {
            if (dictionary == null) {
                return;
            }

            var pairs = (System.Collections.IEnumerable)dictionary;
            foreach (object pair in pairs) {
                object key, value;
                ExtractKeyAndValueFromPair(pair, out key, out value);

                XElement xPair = new XElement(XName_PairElement);

                XElement xKey = new XElement(XName_KeyElement);
                KeyType.WriteToXElement(xKey, key);
                xPair.Add(xKey);

                XElement xValue = new XElement(XName_ValueElement);
                ValueType.WriteToXElement(xValue, value);
                xPair.Add(xValue);

                target.Add(xPair);
            }
        }

        public override object ReadFromXElement(XElement source)
        {
            object dictionary = CreateInstance();

            var xPairs = source.Elements(XName_PairElement);
            foreach (XElement xPair in xPairs) {
                XElement xKey = xPair.Element(XName_KeyElement);
                XElement xValue = xPair.Element(XName_ValueElement);
                object key = KeyType.ReadFromXElement(xKey);
                object value = ValueType.ReadFromXElement(xValue);
                if (key != null) {
                    AddToDictionary(dictionary, key, value);
                }
            }
            return dictionary;
        }

        public override void WriteDefaultValueToXElement(XElement target)
        {
        }

        internal void AddToDictionary(object dictionary, object key, object value)
        {
            Type dictionaryInterface = dictionary.GetType().GetInterface("IDictionary`2");
            MethodInfo addMethod = dictionaryInterface.GetMethod("Add");
            addMethod.Invoke(dictionary, new object[] { key, value });
        }

        internal static void ExtractKeyAndValueFromPair(object pair, out object key, out object value)
        {
            Type pairType = pair.GetType();
            PropertyInfo keyProperty = pairType.GetProperty("Key");
            PropertyInfo valueProperty = pairType.GetProperty("Value");
            key = keyProperty.GetValue(pair, null);
            value = valueProperty.GetValue(pair, null);
        }
    }
}
