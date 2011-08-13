using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ObjectConfigurator.Tests.Test1
{
    class UriItemType : ICustomConfigurationItemType
    {
        public bool AcceptType(string typeFullName)
        {
            return typeFullName == typeof(Uri).FullName;
        }

        public void WriteDefaultValueToXElement(XElement target)
        {
            target.Value = "about:blank";
        }

        public void WriteDefaultValueToXElement(XElement target, object defaultValue)
        {
            target.Value = (string)defaultValue;
        }

        public void WriteToXElement(XElement target, object value)
        {
            target.Value = ((Uri)value).ToString();
        }

        public object ReadFromXElement(XElement source)
        {
            return new Uri(source.Value);
        }

        public ICustomConfigurationValueEditor CreateEditor()
        {
            return new UriEditor();
        }
    }
}
