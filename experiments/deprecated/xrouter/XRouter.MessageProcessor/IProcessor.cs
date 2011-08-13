using XRouter.Management;

namespace XRouter.Processor
{
    /// <summary>
    /// Message Processor.
    /// 
    /// It acts as a container over several processing workers which separates
    /// receiving messages from the actual processing. It is responsible for
    /// receiving the messages from a Dispatcher but delegates the processing
    /// to Processing Providers. Also it takes care of running the workers.
    /// 
    /// It can also provide information on its current load, such as memory
    /// usage, the number of messages waiting for processing etc. Those
    /// information could be useful for the Dispatcher.
    /// 
    /// This is API intended to be used by other components.
    /// </summary>
    /// <seealso cref="XRouter.Processor.IProcessingProvider"/>
    public interface IProcessor : IXRouterComponent
    {
        void Process(Message message);
    }
}
