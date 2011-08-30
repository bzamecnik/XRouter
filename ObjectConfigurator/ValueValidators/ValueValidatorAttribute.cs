using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using ObjectConfigurator.ItemTypes;

namespace ObjectConfigurator.ValueValidators
{
    /// <summary>
    /// Abstract validator of value for configuration items.
    /// </summary>
    [DataContract]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public abstract class ValueValidatorAttribute : Attribute
    {
        /// <summary>
        /// Evaluates if a given value is valid.
        /// </summary>
        /// <param name="value">A value to test.</param>
        /// <param name="errorDescription">In case of error, this is description of the error.</param>
        /// <returns></returns>
        public abstract bool IsValid(object value, out string errorDescription);

        protected bool AreElementsValidIfIsCollection(object value, out string errorDescription, out bool isCollection)
        {
            Type dictionaryInterface = value.GetType().GetInterface(typeof(IDictionary<,>).FullName);
            if (dictionaryInterface != null) {
                isCollection = true;
                foreach (object pair in (System.Collections.IEnumerable)value) {
                    object pairKey, pairValue;
                    DictionaryItemType.ExtractKeyAndValueFromPair(pair, out pairKey, out pairValue);
                    if (!IsValid(pairValue, out errorDescription)) {
                        return false;
                    }
                }
                errorDescription = null;
                return true;
            }

            if (value is System.Collections.IEnumerable) {
                isCollection = true;
                foreach (object element in (System.Collections.IEnumerable)value) {
                    if (!IsValid(element, out errorDescription)) {
                        return false;
                    }
                }
                errorDescription = null;
                return true;
            }

            isCollection = false;
            errorDescription = null;
            return true;
        }
    }
}
