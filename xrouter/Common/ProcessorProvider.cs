using XRouter.Common.ComponentInterfaces;

namespace XRouter.Common
{
    /// <summary>
    /// An object which holds both a reference to a processor component
    /// instance and its name.
    /// </summary>
    public class ProcessorProvider
    {
        /// <summary>
        /// Identifier of the processor component instance.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Reference to the processor component instance.
        /// </summary>
        public IProcessorService Processor { get; private set; }

        public ProcessorProvider(string name, IProcessorService processor)
        {
            Name = name;
            Processor = processor;
        }
    }
}
