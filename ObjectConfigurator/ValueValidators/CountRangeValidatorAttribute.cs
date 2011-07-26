using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectConfigurator.ValueValidators
{
    public class CountRangeValidatorAttribute : ValueValidatorAttribute
    {
        private static readonly string DefaultErrorDescription = "Collection/dictionary has {0} items but items count must be within range {1} and {2}.";
        private static readonly string InvalidValueTypeError = "Value type {0} is not collection/dictionary.";

        public int MinCount { get; private set; }
        public int MaxCount { get; private set; }

        public string ErrorDescription { get; private set; }

        public CountRangeValidatorAttribute(int minCount, int maxCount)
            : this(minCount, maxCount, null)
        {
        }

        public CountRangeValidatorAttribute(int minCount, int maxCount, string errorDescription)
        {
            MinCount = minCount;
            MaxCount = maxCount;
            ErrorDescription = errorDescription ?? DefaultErrorDescription;
        }

        public override bool IsValid(object value, out string errorDescription)
        {
            int count;
            if (value == null) {
                count = 0;
            } else {
                if (value is System.Collections.IEnumerable) {
                    count = ((System.Collections.IEnumerable)value).Cast<object>().Count();
                } else {
                    errorDescription = string.Format(InvalidValueTypeError, value.GetType());
                    return false;
                }
            }

            if ((count >= MinCount) && (count <= MaxCount)) {
                errorDescription = null;
                return true;
            } else {
                errorDescription = string.Format(ErrorDescription, count, MinCount, MaxCount);
                return false;
            }
        }
    }
}
