using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ObjectConfigurator
{
    /// <summary>
    /// Marks a configuration item (field or property) to be managed by
    /// Configurator. Also enables to provide further information such as a
    /// user-friendly item name and description.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
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
