using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using ObjectConfigurator.ItemTypes;

namespace ObjectConfigurator.ValueValidators
{
    /// <summary>
    /// Configuration item validator restricting string to a certain regular expression.
    /// If applied on a collection, items of this collection will be validated.
    /// If applied on a dictionary, values of this dictionary will be validated.
    /// </summary>
    [DataContract]
    public class RegexValidatorAttribute : ValueValidatorAttribute
    {
        private static readonly string DefaultErrorDescription = "Value {0} does not match regular expresson {1}.";

        /// <summary>
        /// Regular expression to which value must match.
        /// </summary>
        [DataMember]
        public string RegularExpression { get; private set; }

        /// <summary>
        /// Custom error description for user if this validation fails. If null, a default error description is used.
        /// </summary>
        [DataMember]
        public string ErrorDescription { get; private set; }

        public RegexValidatorAttribute(string regularExpression)
            : this(regularExpression, null)
        {
        }

        public RegexValidatorAttribute(string regularExpression, string errorDescription)
        {
            RegularExpression = regularExpression;
            ErrorDescription = errorDescription ?? DefaultErrorDescription;
        }

        public override bool IsValid(object value, out string errorDescription)
        {
            if (value == null) {
                errorDescription = string.Format(ErrorDescription, "(null)", RegularExpression);
            }

            bool isCollection;
            bool areElementsValid = AreElementsValidIfIsCollection(value, out errorDescription, out isCollection);
            if (isCollection) {
                return areElementsValid;
            }

            string str = value.ToString();
            if (Regex.IsMatch(str, RegularExpression)) {
                errorDescription = null;
                return true;
            } else {
                errorDescription = string.Format(ErrorDescription, str, RegularExpression);
                return false;
            }
        }
    }
}
