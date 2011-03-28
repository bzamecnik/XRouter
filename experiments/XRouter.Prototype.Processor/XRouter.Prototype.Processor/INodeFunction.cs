using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Prototype.Processor
{
    interface INodeFunction
    {
        void Initialize();

        void Evaluate(Token token);
    }
}
