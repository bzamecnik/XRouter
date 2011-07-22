using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectConfigurator.ValueValidators
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public abstract class ValueValidatorAttribute : Attribute
    {
        public abstract bool IsValid(object value, out string errorDescription);
    }
}
