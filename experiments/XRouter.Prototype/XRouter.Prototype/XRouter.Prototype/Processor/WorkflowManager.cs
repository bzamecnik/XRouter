using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Prototype.CoreTypes;
using XRouter.Prototype.CoreServices;

namespace XRouter.Prototype.Processor
{
    /// <summary>
    /// Tato trida bude spravovat kolekci aktualnich workflow systemu. 
    /// </summary>
    class WorkflowManager
    {
        private List<Workflow> list = new List<Workflow>();

        /// <summary>
        /// Vraci id aktualni (nejnovejsi) verze workflow. 
        /// </summary>
        public Int32 ActualVersion
        {
            get { return 1; }
        }

        /// <summary>
        /// V konfiguraci by mely byt informace o vsech verzich workflow, 
        /// ktere jeste mohou byt v systemu. 
        /// </summary>
        /// <param name="config"></param>
        public WorkflowManager(ApplicationConfiguration config)
        {          
            // pro vsechny workflow v config
            this.AddWorkflow(null);            
        }
        
        public void AddWorkflow(ApplicationConfiguration config)
        {                               
            lock (this.list)
            {
                list.Add(new Workflow(config));
            }
        }

        /// <summary>
        /// Na zaklade informaci v tokenu vybira nasleduji workflow node. 
        /// Pokud vrati null, pak uz neni zadny naslednik. 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public WorkflowNode GetNextNode(Token token)
        {
            WorkflowNode result = null;

            lock (this.list)
            {
                // vybere workflow dle verze
                Workflow workflow = null;
                foreach (Workflow wf in this.list)
                {
                    if (token.WorkflowState.WorkflowVersion == wf.Version)
                    {
                        workflow = wf;
                        break;
                    }
                }

                // vybere nasledujici node dle step
                if (token.WorkflowState.Step == -1)
                {
                    result = workflow.RootNode;
                }
                else
                {
                    WorkflowEdge edge = workflow.GetEdge(token.WorkflowState.Step);
                    result = edge.To;
                }
            }

            return result;
        }
    }
}
