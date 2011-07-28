using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace ObjectConfigurator.ItemTypes
{
    [DataContract]
    [KnownType(typeof(BasicItemType))]
    [KnownType(typeof(EnumItemType))]
    [KnownType(typeof(CollectionItemType))]
    [KnownType(typeof(DictionaryItemType))]
    [KnownType(typeof(CustomItemType))]
    class CollectionItemType : ItemType
    {
        internal static readonly XName XName_CollectionElement = XName.Get("element");

        [DataMember]
        public ItemType ElementType { get; private set; }

        public CollectionItemType(Type clrType, ItemType elementType)
            : base(clrType)
        {
            ElementType = elementType;
        }

        public override void WriteToXElement(XElement target, object value)
        {
            if (value == null) {
                return;
            }
            var collection = (System.Collections.IEnumerable)value;
            foreach (object element in collection) {
                XElement xElement = new XElement(XName_CollectionElement);
                ElementType.WriteToXElement(xElement, element);
                target.Add(xElement);
            }
        }

        public override object ReadFromXElement(XElement source)
        {
            object collection = CreateInstance();

            var xElements = source.Elements(XName_CollectionElement);
            foreach (XElement xElement in xElements) {
                object element = ElementType.ReadFromXElement(xElement);
                if (element != null) {
                    AddToCollection(collection, element);
                }
            }
            return collection;
        }

        public override void WriteDefaultValueToXElement(XElement target)
        {
        }

        internal void AddToCollection(object collection, object element)
        {
            Type collectionInterface = collection.GetType().GetInterface("ICollection`1");
            MethodInfo addMethod = collectionInterface.GetMethod("Add");
            addMethod.Invoke(collection, new object[] { element });
        }
    }
}
