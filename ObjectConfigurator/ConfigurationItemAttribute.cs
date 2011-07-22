using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ObjectConfigurator
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ConfigurationItemAttribute : Attribute
    {
        public string UserName { get; set; }
        public string UserDescription { get; set; }
        public object DefaultValue { get; set; }

        public ConfigurationItemAttribute(string userName, string userDescription, object defaultValue)
        {
            UserName = userName;
            UserDescription = userDescription;
            DefaultValue = defaultValue;
        }
    }
}
