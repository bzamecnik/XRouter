using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Prototype.CoreTypes;

namespace XRouter.Prototype.CoreTypes
{
    interface IProcessor : IComponent
    {
        double GetUtilization();

        void AddWork(Token token);
    }
}
