using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Prototype.CoreTypes;
using XRouter.Prototype.CoreServices;
using System.Threading;

namespace XRouter.Prototype.Processor
{
    class ProcessorImplementation : IProcessor
    {
        public XmlReduction ConfigReduction { get { return new XmlReduction(); } }

        // fronta tokenu cekajicich na zpracovani
        private Queue<Token> queue = new Queue<Token>();
        
        private Thread[] workers = null;

        private WorkflowManager wfManager = null;
       
        public void Initalize(ApplicationConfiguration config, ICentralComponentServices services)
        {

            // vytvori pole workeru        
            Int32 workersCount = 5;
            this.workers = new Thread[workersCount];            

            // inicializuje workflows dle konfigurace
            this.wfManager = new WorkflowManager(config);            
        }

        public void ChangeConfig(ApplicationConfiguration config)
        {
            this.wfManager.AddWorkflow(config);
        }

        public void Start()
        { 
            // instancuje a spusti workery
            for (Int32 i = 0; i < this.workers.Length; i++)
            {
                this.workers[i] = new Thread(new ParameterizedThreadStart(this.Processing));
                this.workers[i].Start();
            }
        }

        public void Stop()
        {
            // ukonci workery (zatim provizorne)            
            for (Int32 i = 0; i < this.workers.Length; i++)
            {             
                this.workers[i].Abort();
            }
        }

        public double GetUtilization()
        {
            return 0.5d;
        }
        
        public void AddWork(Token token)
        {
            lock (this.queue)
            {
                this.queue.Enqueue(token);
            }
        }

        private void Processing(Object data)
        {
            try
            {
                while (true)
                {
                    // vybere token z kolekce
                    Token token = null;
                    lock (this.queue)
                    {
                        if (this.queue.Count > 0)
                        {
                            token = this.queue.Dequeue();
                        }
                    }

                    if (token == null)
                    {
                        continue;
                    }

                    // token provede nekolik kroku
                    Boolean isDone = false;
                    Int32 MaxStepsToDoAtOnce = 5;
                    for (int i = 0; i < MaxStepsToDoAtOnce; i++)
                    {
                        if (!DoStep(token))
                        {
                            isDone = true;
                            break;
                        }
                    }

                    // vrati token do kolekce
                    if (!isDone)
                    {
                        if (token.State != TokenState.Processed)
                        {
                            this.AddWork(token);
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            { }
        }

        private Boolean DoStep(Token token)
        {
            if (token.State == TokenState.Received)
            {
                // zmeni stav tokenu a nastavi dalsi informace (verze workflow, identifikator processoru apod.)
                token.State = TokenState.InProcessor;                
                token.WorkflowState.WorkflowVersion = this.wfManager.ActualVersion;                
            }

            // ziska nasledujici node
            WorkflowNode node = this.wfManager.GetNextNode(token);
            if (node == null)
            {
                token.State = TokenState.Processed;
                return true;
            }

            if (node.Sensitive)
            {
                if (token.IsNodeVisited(node.Id))
                {
                    // chyba, token nemuze znovu node projit
                    // jedna z vyjimek workflow, ktera je nejakym zpusobem fatalni
                }
                token.SaveVisitedNode(node.Id);              
            }

            // provede kod definovany nodem (funkce aktualizuje token)
            INodeFunction func = node.Function;        
            func.Evaluate(token);
                        
            return false;
        }

    }
}
