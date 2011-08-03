using System;
using System.Runtime.Serialization;

namespace ObjectConfigurator.ValueValidators
{
    [DataContract]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public abstract class ValueValidatorAttribute : Attribute
    {
        public abstract bool IsValid(object value, out string errorDescription);
    }
}
