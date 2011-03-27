using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Prototype.Processor
{
    /// <summary>
    /// List workflow grafu. 
    /// </summary>
    class Terminator : INodeFunction
    {
        private String name = null;
        
        public String Name
        {
            get { return "Terminator." + name; }
        }
               
        public void Initialize(CoreTypes.ApplicationConfiguration config)
        {
            // this.name =           
        }

        public void Evaluate(CoreTypes.Token token)
        {
            lock (this)
            {
                
            }            
        }
    }
}
