using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectConfigurator;
using System.Xml.Linq;

namespace XRouter.Common.MessageFlowConfig
{
    public class TokenSelectionConfigurationItemType : ICustomConfigurationItemType
    {
        private Func<ICustomConfigurationValueEditor> editorFactory;

        public TokenSelectionConfigurationItemType(Func<ICustomConfigurationValueEditor> editorFactory = null)
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
            return typeFullName == typeof(TokenSelection).FullName;
        }

        public void WriteDefaultValueToXElement(XElement target)
        {
            TokenSelection defaultValue = new TokenSelection();
            WriteToXElement(target, defaultValue);
        }

        public void WriteDefaultValueToXElement(XElement target, object defaultValue)
        {
            if (defaultValue == null) {
                WriteDefaultValueToXElement(target);
                return;
            }

            if (!(defaultValue is string)) {
                throw new InvalidOperationException("Default value for configuration item of type TokenSelection is not string.");
            }
            string pattern = (string)defaultValue;
            TokenSelection value = new TokenSelection(pattern);
            WriteToXElement(target, value);
        }

        public void WriteToXElement(XElement target, object value)
        {
            TokenSelection tokenSelection = (TokenSelection)value;
            target.Value = tokenSelection.SelectionPattern;
        }

        public object ReadFromXElement(XElement source)
        {
            string pattern = source.Value;
            return new TokenSelection(pattern);
        }

        public ICustomConfigurationValueEditor CreateEditor()
        {
            return editorFactory();
        }
    }
}
