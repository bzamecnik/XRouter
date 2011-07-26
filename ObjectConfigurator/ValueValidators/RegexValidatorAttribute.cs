using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ObjectConfigurator.ValueValidators
{
    public class RegexValidatorAttribute : ValueValidatorAttribute
    {
        private static readonly string DefaultErrorDescription = "Value {0} does not match regular expresson {1}.";

        public string RegularExpression { get; private set; }

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
