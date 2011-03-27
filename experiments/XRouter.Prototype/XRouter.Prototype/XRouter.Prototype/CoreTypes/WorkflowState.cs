using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Prototype.CoreTypes
{
    class WorkflowState
    {
        public int WorkflowVersion { get; set; }

        /// <summary>
        /// Identifikator hrany v grafu. Pokud je -1, pak jeste neni ve workflow. 
        /// </summary>
        public int Step { get; set; }

        public string AssignedProcessor { get; set; }

        public DateTime LastResponseFromProcessor { get; set; }

        public WorkflowState()
        {
            this.Step = -1;
        }
    }
}
