using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Prototype.CoreTypes;
using XRouter.Prototype.CoreServices;

namespace XRouter.Prototype.Processor
{
    /// <summary>
    /// Content Based Router.
    /// </summary>
    class CBR : INodeFunction
    {
        private String name = null;
        
        public String Name
        {
            get { return "CBR." + name; }
        }

        public void Initialize(ApplicationConfiguration config)
        {
           // this.name = 
        }

        /// <summary>
        /// Provede směrování, tj. vybere id další hrany a aktualizuje token. Na základě
        /// získané hodnoty aktualizuje token.step.
        /// </summary>
        /// <param name="token"></param>
        public void Evaluate(Token token)
        {
            lock (this)
            {

            }        
        }
    }
}
