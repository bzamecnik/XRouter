using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Prototype.Processor
{
    class WorkflowNode
    {
        /// <summary>
        /// V ramci workflow jednoznacna identifikace nodu. 
        /// </summary>
        public Int32 Id = -1;

        /// <summary>
        /// Priznak urcuje, ze je tento node "citlivy", tj. pri obnoveni 
        /// workflow nesmi token nodem znovu projit. Citlivost nodu urcuje
        /// v konfiguraci uzivatel. 
        /// </summary>
        public Boolean Sensitive { private set; get; }

        /// <summary>
        /// Implementace nodu. 
        /// </summary>
        public INodeFunction Function { private set; get; }
    }
}
