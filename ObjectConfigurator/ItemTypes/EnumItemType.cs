using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace ObjectConfigurator.ItemTypes
{
    [DataContract]
    class EnumItemType : ItemType
    {
        [DataMember]
        public ReadOnlyCollection<string> ValueNames { get; private set; }

        public EnumItemType(Type clrType, IEnumerable<string> valueNames)
            : base(clrType)
        {
            ValueNames = new ReadOnlyCollection<string>(valueNames.ToArray());
        }

        public override void WriteToXElement(XElement target, object value)
        {
            string content = value.ToString();
            target.SetValue(content);
        }

        public override object ReadFromXElement(XElement source)
        {
            Type clrType = GetClrType();
            string value = source.Value;
            object result = Enum.Parse(clrType, value);
            return result;
        }

        public override void WriteDefaultValueToXElement(XElement target)
        {
            target.Value = ValueNames.First();
        }
    }
}
