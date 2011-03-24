using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchematronVal
{
    /// <summary>
    /// Returns detailed information related to the assertion.
    /// </summary>
    public sealed class AssertionInfo
    {
        internal AssertionInfo()
        { }
        
        /// <summary>
        /// Indicates that the assertion is report. 
        /// </summary>
        public Boolean IsReport { internal set; get; }
        
        /// <summary>
        /// Gets the assertion pattern id.
        /// </summary>
        public String PatternId { internal set; get; }

        /// <summary>
        /// Gets the assertion rule id.
        /// </summary>
        public String RuleId { internal set; get; }

        /// <summary>
        /// Gets the assertion rule context.
        /// </summary>
        public String RuleContext { internal set; get; }

        /// <summary>
        /// Gets the assertion id.
        /// </summary>
        public String AssertionId { internal set; get; }

        /// <summary>
        /// Gets the assertion test.
        /// </summary>
        public String AssertionTest { internal set; get; }

        /// <summary>
        /// Gets the node line number (pokud byla instance dodane s line information).
        /// </summary>
        public Int32 LineNumber { internal set; get; }

        /// <summary>
        /// Gets the node line position (pokud byla instance dodane s line information).
        /// </summary>
        public Int32 LinePosition { internal set; get; }

        /// <summary>
        /// Gets the node xpath location.
        /// </summary>
        public String Location { internal set; get; }

        /// <summary>
        /// Gets the assertion user-message (name and value-of
        /// elements are substitued with selected values).      
        /// </summary>
        public String UserMessage { internal set; get; }

        public override string ToString()
        {
            return this.UserMessage;
        }
    }
}
