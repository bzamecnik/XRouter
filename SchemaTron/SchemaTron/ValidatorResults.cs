using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchemaTron
{
    /// <summary>
    /// Vraci detailni vysledky validace.
    /// </summary>
    public sealed class ValidatorResults
    {       
        internal ValidatorResults()
        {
            this.IsValid = true;
            this.violatedAssertions = new List<AssertionInfo>(); 
        }

        /// <summary>
        /// Urcuje, jestli je dana XML instance proti schematu validni. Pokud je 
        /// vyvolana alespon jedna assertion, pak je instance nevalidni. 
        /// </summary>
        public Boolean IsValid { internal set; get; }

        internal List<AssertionInfo> violatedAssertions = null;

        /// <summary>
        /// Vraci informace o vyvolanych assertions. Pokud je XML instance validni,
        /// pak je vraceno prazdne pole.
        /// </summary>
        public AssertionInfo[] ViolatedAssertions
        {
            get { return violatedAssertions.ToArray(); }
        }
       
        public override string ToString()
        {
            return String.Format("IsValid='{0}' ViolatedAssertions='{1}'", this.IsValid, this.ViolatedAssertions.Length);
        }

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
