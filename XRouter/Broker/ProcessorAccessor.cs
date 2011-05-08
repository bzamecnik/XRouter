using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;

namespace XRouter.Broker
{
    class ProcessorAccessor : ComponentAccessor
    {
        public ProcessorAccessor(string componentName, ApplicationConfiguration configuration)
            : base(componentName, configuration)
        {
        }

        public double GetUtilization()
        {
            IProcessorService processor = GetComponent<IProcessorService>();
            double utilization = processor.GetUtilization();
            return utilization;
        }

        public void AddWork(Token token)
        {
            IProcessorService processor = GetComponent<IProcessorService>();
            processor.AddWork(token);
        }
    }
}
