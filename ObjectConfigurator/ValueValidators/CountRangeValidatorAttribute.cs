using System.Linq;
using System.Runtime.Serialization;

namespace ObjectConfigurator.ValueValidators
{
    /// <summary>
    /// Configuration item validator restricting count of items of a collection or a dictionary.
    /// </summary>
    [DataContract]
    public class CountRangeValidatorAttribute : ValueValidatorAttribute
    {
        private static readonly string DefaultErrorDescription = "Collection/dictionary has {0} items but items count must be within range {1} and {2}.";
        private static readonly string InvalidValueTypeError = "Value type {0} is not collection/dictionary.";

        /// <summary>
        /// Minimal allowed count of items.
        /// </summary>
        [DataMember]
        public int MinCount { get; private set; }

        /// <summary>
        /// Maximal allowed count of items.
        /// </summary>
        [DataMember]
        public int MaxCount { get; private set; }

        /// <summary>
        /// Custom error description for user if this validation fails. If null, a default error description is used.
        /// </summary>
        [DataMember]
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
