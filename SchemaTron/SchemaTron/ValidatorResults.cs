using System;
using System.Collections.Generic;

namespace SchemaTron
{
    // TODO: probably rename to ValidationResult

    /// <summary>
    /// Detailed results of a validation.
    /// </summary>
    public sealed class ValidatorResults
    {
        /// <summary>
        /// The list of assertions violated during the validation process.
        /// </summary>
        public List<AssertionInfo> ViolatedAssertionsList { get; set; }

        /// <summary>
        /// Indicates whether a given XML document instance is valid with
        /// respect to the validator's schema. In case of any violated
        /// assertion the XML instance cannot be valid.
        /// </summary>
        public bool IsValid { get; set; }

        public ValidatorResults()
        {
            this.IsValid = true;
            this.ViolatedAssertionsList = new List<AssertionInfo>();
        }

        /// <summary>
        /// Gets the information on assertions violated during the validation.
        /// In case the XML document instance is valid the array of assertions
        /// is empty.
        /// </summary>
        public IEnumerable<AssertionInfo> ViolatedAssertions
        {
            // TODO: is it better to return an array or an unmodifiable list?
            get { return ViolatedAssertionsList; }
        }

        /// <summary>
        /// Returns a System.String which represent the current
        /// ValidatorResults instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("IsValid='{0}' ViolatedAssertions='{1}'", this.IsValid, this.ViolatedAssertionsList.Count);
        }

        /// <summary>
        /// Converts the list of violated assertions into a list of
        /// user-readable error messages.
        /// </summary>
        /// <returns>A list of error messages about the violated assertions
        /// </returns>
        public IEnumerable<string> GetMessages()
        {
            List<string> messages = new List<string>();
            foreach (AssertionInfo info in this.ViolatedAssertionsList)
            {
                messages.Add(info.UserMessage);
            }

            return messages;
        }
    }
}
