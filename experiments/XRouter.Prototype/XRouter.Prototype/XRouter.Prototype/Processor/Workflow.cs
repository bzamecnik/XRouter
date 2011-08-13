using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Prototype.CoreTypes;
using XRouter.Prototype.CoreServices;

namespace XRouter.Prototype.Processor
{
    class Workflow
    {
        public Int32 Version { set; get; }

        private Dictionary<Int32, WorkflowNode> nodes = new Dictionary<Int32, WorkflowNode>();
        
        private Dictionary<Int32, WorkflowEdge> edges = new Dictionary<Int32, WorkflowEdge>();

        public WorkflowNode RootNode { private set; get; }

        /// <summary>
        /// Z aplikační konfigurace deserializuje workflow do objektového modelu.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public Workflow(ApplicationConfiguration config)
        {           
            // kod deserializace
            this.Version = 1;           
        }
               
        public WorkflowEdge GetEdge(Int32 id)
        {
            return edges[id];
        }        
    }
}
