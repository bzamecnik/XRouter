using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Prototype.Processor
{
    class Transform : INodeFunction
    {
        public void Initialize()
        {
            
        }

        public void Evaluate(Token token)
        {
            Logger.LogInfo("Transformuji zpravu.");
        }
    }
}
