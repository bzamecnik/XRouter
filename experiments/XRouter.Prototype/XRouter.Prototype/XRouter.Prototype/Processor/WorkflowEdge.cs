using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Prototype.Processor
{
    class WorkflowEdge
    {
        /// <summary>
        /// V ramci workflow jednoznacny identifikator hrany. 
        /// </summary>
        public Int32 Id = -1;
        
        /// <summary>
        /// Parent node, pokud je null, pak je To koren. 
        /// </summary>
        public WorkflowNode From { set; get; }

        /// <summary>
        /// Child node, pokud null, pak je From list. 
        /// </summary>
        public WorkflowNode To { set; get; }
    }
}
