using XRouter.Common;
using XRouter.Common.MessageFlowConfig;

namespace XRouter.Processor.MessageFlowParts
{
    /// <summary>
    /// Represents a single node of the message flow graph. This is a base class
    /// with common functionality for concrete message flow nodes.
    /// </summary>
    /// <remarks>
    /// A node instance is specific to a single processor.
    /// </remarks>
    abstract class Node
    {
        /// <summary>
        /// Name of the node.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Reference to the processor in which the node works.
        /// </summary>
        protected ProcessorServiceForNode ProcessorService { get; private set; }

        /// <summary>
        /// Initializes the node.
        /// </summary>
        /// <remarks>
        /// This method has to be called before the node can be used!
        /// </remarks>
        /// <param name="configuration"></param>
        /// <param name="processor"></param>
        public void Initialize(NodeConfiguration configuration, ProcessorService processor)
        {
            Name = configuration.Name;
            ProcessorService = new ProcessorServiceForNode(processor, this);
            InitializeCore(configuration);
        }

        /// <summary>
        /// Initializes a concrete implementation of the node.
        /// </summary>
        /// <remarks>Indened to be overridden in the derived class.</remarks>
        /// <param name="configuration"></param>
        public abstract void InitializeCore(NodeConfiguration configuration);

        /// <summary>
        /// Evaluates the node with the given token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>
        /// Name of the next node or null if the processing is terminated.
        /// </returns>
        public abstract string Evaluate(ref Token token);
    }
}
