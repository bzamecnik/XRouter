using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common.ComponentInterfaces
{
    public interface IProcessorService : IComponentService
    {
        void Start(string componentName, IBrokerServiceForProcessor brokerService);
        void Stop();

        double GetUtilization();

        void AddWork(Token token);
    }
}
