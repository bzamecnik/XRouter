using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectConfigurator;
using System.Xml.Linq;

namespace XRouter.Common.Xrm
{
    public class XrmUriConfigurationItemType : ICustomConfigurationItemType
    {
        private Func<ICustomConfigurationValueEditor> editorFactory;

        public XrmUriConfigurationItemType(Func<ICustomConfigurationValueEditor> editorFactory = null)
        {
            if (editorFactory == null) {
                editorFactory = delegate {
                    throw new InvalidOperationException("Editor factory has not been provided.");
                };
            }
            this.editorFactory = editorFactory;
        }

        public bool AcceptType(string typeFullName)
        {
            return typeFullName == typeof(XrmUri).FullName;
        }

        public void WriteDefaultValueToXElement(XElement target)
        {
            XrmUri defaultValue = new XrmUri();
            WriteToXElement(target, defaultValue);
        }

        public void WriteDefaultValueToXElement(XElement target, object defaultValue)
        {
            if (defaultValue == null) {
                WriteDefaultValueToXElement(target);
                return;
            }

            if (!(defaultValue is string)) {
                throw new InvalidOperationException("Default value for configuration item of type XrmUri must be string.");
            }
            string xpath = (string)defaultValue;
            XrmUri value = new XrmUri(xpath);
            WriteToXElement(target, value);
        }

        public void WriteToXElement(XElement target, object value)
        {
            XrmUri xrmUri = (XrmUri)value;
            target.Value = xrmUri.XPath;
        }

        public object ReadFromXElement(XElement source)
        {
            string xpath = source.Value;
            return new XrmUri(xpath);
        }

        public ICustomConfigurationValueEditor CreateEditor()
        {
            return editorFactory();
        }
    }
}
