using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common
{
    public interface IProcessorService : IComponentService
    {
        double GetUtilization();

        void AddWork(Token token);
    }
}
