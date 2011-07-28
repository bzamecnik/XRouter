using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace ObjectConfigurator.ItemTypes
{
    [DataContract]
    class CustomItemType : ItemType
    {
        public CustomItemType(Type clrType)
            : base(clrType)
        {
        }

        public override void WriteToXElement(XElement target, object value)
        {
            if (value != null) {
                var customType = GetCustomType();
                customType.WriteToXElement(target, value);
            } else {
                target.SetAttributeValue(XName.Get("isNull"), "true");
            }
        }

        public override object ReadFromXElement(XElement source)
        {
            if (source.Attribute(XName.Get("isNull")).Value == "true") {
                return null;
            }

            var customType = GetCustomType();
            customType.WriteDefaultValueToXElement(source);
            return customType.ReadFromXElement(source);
        }

        public override void WriteDefaultValueToXElement(XElement target)
        {
            var customType = GetCustomType();
            customType.WriteDefaultValueToXElement(target);
        }

        public ICustomItemType GetCustomType()
        {
            ICustomItemType result = Configurator.CustomItemTypes.FirstOrDefault(r => r.AcceptType(ClrTypeFullName));
            if (result == null) {
                throw new InvalidOperationException(string.Format("Cannot find custom type for {0}.", ClrTypeFullName));
            }
            return result;
        }
    }
}
