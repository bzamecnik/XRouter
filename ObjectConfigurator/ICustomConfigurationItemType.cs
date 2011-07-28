using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ObjectConfigurator
{
    public interface ICustomConfigurationItemType
    {
        bool AcceptType(string typeFullName);

        void WriteDefaultValueToXElement(XElement target);
        void WriteDefaultValueToXElement(XElement target, object defaultValue);

        void WriteToXElement(XElement target, object value);
        object ReadFromXElement(XElement source);

        ICustomConfigurationValueEditor CreateEditor();
    }
}
