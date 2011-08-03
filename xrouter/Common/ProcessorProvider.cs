using XRouter.Common.ComponentInterfaces;

namespace XRouter.Common
{
    public class ProcessorProvider
    {
        public string Name { get; private set; }

        public IProcessorService Processor { get; private set; }

        public ProcessorProvider(string name, IProcessorService processor)
        {
            Name = name;
            Processor = processor;
        }
    }
}
