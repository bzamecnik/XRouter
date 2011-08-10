using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectConfigurator;
using System.Xml.Linq;

namespace XRouter.Common
{
    public class UriConfigurationItemType : ICustomConfigurationItemType
    {
        private Func<ICustomConfigurationValueEditor> editorFactory;

        public UriConfigurationItemType(Func<ICustomConfigurationValueEditor> editorFactory = null)
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
            return typeFullName == typeof(Uri).FullName;
        }

        public void WriteDefaultValueToXElement(XElement target)
        {
            Uri defaultValue = new Uri("about:blank");
            WriteToXElement(target, defaultValue);
        }

        public void WriteDefaultValueToXElement(XElement target, object defaultValue)
        {
            if (defaultValue == null) {
                WriteDefaultValueToXElement(target);
                return;
            }

            if (!(defaultValue is string)) {
                throw new InvalidOperationException("Default value for configuration item of type Uri must be string.");
            }
            Uri value = new Uri((string)defaultValue);
            WriteToXElement(target, value);
        }

        public void WriteToXElement(XElement target, object value)
        {
            Uri uri = (Uri)value;
            target.Value = uri.ToString();
        }

        public object ReadFromXElement(XElement source)
        {
            Uri result = new Uri(source.Value);
            return result;
        }

        public ICustomConfigurationValueEditor CreateEditor()
        {
            return editorFactory();
        }
    }
}
