using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace ObjectConfigurator
{
    class EnumItemType : ItemType
    {
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
    }
}
