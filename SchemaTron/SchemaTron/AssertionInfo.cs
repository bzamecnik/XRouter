using System;

namespace SchemaTron
{
    /// <summary>
    /// Contains detailed information related to an assertion.
    /// </summary>
    /// <remarks>
    /// This class is a data-transfer object (DTO).
    /// </remarks>
    public sealed class AssertionInfo
    {
        /// <summary>
        /// Prevent creating instances outside this assembly.
        /// </summary>
        internal AssertionInfo()
        { }

        /// <summary>
        /// Indicates whether the assertion is a report.
        /// </summary>
        public Boolean IsReport { internal set; get; }

        /// <summary>
        /// Represents the assertion pattern identifier.
        /// </summary>
        public String PatternId { internal set; get; }

        /// <summary>
        /// Represents the assertion rule identifier.
        /// </summary>
        public String RuleId { internal set; get; }

        /// <summary>
        /// Represents the assertion rule context.
        /// </summary>
        public String RuleContext { internal set; get; }

        /// <summary>
        /// Represents the assertion identifier.
        /// </summary>
        public String AssertionId { internal set; get; }

        /// <summary>
        /// Represents the assertion test.
        /// </summary>
        public String AssertionTest { internal set; get; }

        /// <summary>
        /// Represents the number of the line where the node was located.
        /// </summary>
        /// <remarks>
        /// Only in case the validated XML document instance contained the
        /// line information.
        /// </remarks>
        public Int32 LineNumber { internal set; get; }

        /// <summary>
        /// Represents the position of the node on a line.
        /// </summary>
        /// <remarks>
        /// Only in case the validated XML document instance contained the
        /// line information.
        /// </remarks>
        public Int32 LinePosition { internal set; get; }

        /// <summary>
        /// Represents the XPath location of the node.
        /// </summary>
        public String Location { internal set; get; }

        /// <summary>
        /// Represents the assertion user message with the <c>name</c> and
        /// <c>value-of</c> elements substitued with selected values.
        /// </summary>
        public String UserMessage { internal set; get; }

        /// <summary>
        /// Returns a System.String which represent the current AssertionInfo
        /// instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.UserMessage;
        }
    }
}
