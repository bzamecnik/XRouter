using System;
using System.Collections.Generic;

namespace SchemaTron
{
    /// <summary>
    /// Detailed results of a validation.
    /// </summary>
    public sealed class ValidatorResults
    {
        internal ValidatorResults()
        {
            this.IsValid = true;
            this.violatedAssertions = new List<AssertionInfo>();
        }

        /// <summary>
        /// Indicates whether a given XML document instance is valid with
        /// respect to the validator's schema. In case of any violated
        /// assertion the XML instance cannot be valid.
        /// </summary>
        public Boolean IsValid { internal set; get; }

        /// <summary>
        /// The list of assertions violated during the validation process.
        /// </summary>
        internal List<AssertionInfo> violatedAssertions = null;

        /// <summary>
        /// Gets the information on assertions violated during the validation.
        /// In case the XML document instance is valid the array of assertions
        /// is empty.
        /// </summary>
        public AssertionInfo[] ViolatedAssertions
        {
            // TODO: is it better to return list or array?
            get { return violatedAssertions.ToArray(); }
        }

        /// <summary>
        /// Returns a System.String which represent the current
        /// ValidatorResults instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("IsValid='{0}' ViolatedAssertions='{1}'", this.IsValid, this.ViolatedAssertions.Length);
        }

        /// <summary>
        /// Converts the list of violated assertions into a list of
        /// user-readable error messages.
        /// </summary>
        /// <returns>A list of error messages about the violated assertions
        /// </returns>
        internal String[] GetMessages()
        {
            List<String> messages = new List<String>();
            foreach (AssertionInfo info in this.violatedAssertions)
            {
                messages.Add(info.UserMessage);
            }
            return messages.ToArray();
        }
    }
}
