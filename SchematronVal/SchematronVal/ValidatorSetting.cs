using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchematronVal
{
    /// <summary>
    /// Argumenty validatoru.
    /// </summary>
    public sealed class ValidatorSetting
    {
        /// <summary>
        /// Implementace dodani included Schematron elementu. Pokud neni nastaveno, pak je pouzita 
        /// vychozi implementace.
        /// </summary>
        public InclusionsResolver InclusionsResolver { set; get; }

        /// <summary>
        /// Urceni phase nad kterou se bude provadet validace (napr. pro podporu progresivni validace). 
        /// Phase musi byt specifikovana v Schematron schematu. Identifikatory #ALL a #DEFAULT jsou 
        /// podporovany.
        /// </summary>
        public String Phase { set; get; }
       
        public ValidatorSetting()
        {
            this.Phase = "#ALL";
            this.Preprocessing = true;
        }

        internal Boolean Preprocessing { set; get; }
    }
}
