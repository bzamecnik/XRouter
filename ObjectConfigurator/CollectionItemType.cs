using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;

namespace ObjectConfigurator
{
    class CollectionItemType : ItemType
    {
        public ItemType ElementType { get; private set; }

        public CollectionItemType(Type clrType, ItemType elementType)
            : base(clrType)
        {
            ElementType = elementType;
        }

        public override void WriteToXElement(XElement target, object value)
        {
            var collection = (System.Collections.IEnumerable)value;
            foreach (object element in collection) {
                XElement xElement = new XElement(XName.Get("element"));
                ElementType.WriteToXElement(xElement, element);
                target.Add(element);
            }
        }

        public override object ReadFromXElement(XElement source)
        {
            Type clrType = GetClrType();
            object collection = Activator.CreateInstance(clrType, true);
            MethodInfo addMethod = typeof(ICollection<>).GetMethod("Add");

            var xElements = source.Elements(XName.Get("element"));
            foreach (XElement xElement in xElements) {
                object element = ElementType.ReadFromXElement(xElement);
                addMethod.Invoke(collection, new object[] { element });
            }
            return collection;
        }
    }
}
