using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Prototype.CoreTypes;
using XRouter.Prototype.CoreServices;

namespace XRouter.Prototype.Processor
{
    /// <summary>
    /// Libovolná akce.
    /// </summary>
    class Action : INodeFunction
    {
        private INodeFunction func = null;
      
        public String Name
        {
            get { return "Action." + func.Name; }
        }

        /// <summary>
        /// Vytvori instanci akce, jejiz implementace je definovana v pluginu
        /// </summary>
        /// <param name="config"></param>
        public void Initialize(ApplicationConfiguration config)
        {
            func.Initialize(config);
        }
       
        public void Evaluate(Token token)
        {
            lock (this)
            {
                func.Evaluate(token);
            }
        }        
    }
}
