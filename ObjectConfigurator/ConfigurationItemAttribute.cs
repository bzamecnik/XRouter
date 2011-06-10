using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectConfigurator
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ConfigurationItemAttribute : Attribute
    {
        public string UserName { get; set; }
        public string UserDescription { get; set; }

        public ConfigurationItemAttribute()
        {
        }

        public ConfigurationItemAttribute(string userDescription)
        {
            UserDescription = userDescription;
        }

        public ConfigurationItemAttribute(string userName, string userDescription)
        {
            UserName = userName;
            UserDescription = userDescription;
        }
    }
}
