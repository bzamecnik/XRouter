using System;
using XRouter.Common;
using XRouter.Common.ComponentInterfaces;

namespace XRouter.Broker
{
    /// <summary>
    /// A proxy for accessing a processor. It is able to cache some frequently
    /// queried values such as its utilization.
    /// </summary>
    /// <seealso cref="XRouter.Common.ComponentInterfaces.IProcessorService"/>
    class ProcessorAccessor : ComponentAccessor
    {
        private IProcessorService processor;

        public ProcessorAccessor(string componentName, IProcessorService processor, ApplicationConfiguration configuration)
            : base(componentName, processor, configuration)
        {
            this.processor = processor;
        }

        public void AddWork(Token token)
        {
            processor.AddWork(token);
        }
    }
}
