using XRouter.Management;

namespace XRouter.Processor
{
    /// <summary>
    /// Message Processing Provider.
    /// 
    /// A component which actually processes messages, eg. according to
    /// a workflow.
    /// 
    /// Indended to be used by Message Processor workers.
    /// </summary>
    /// <seealso cref="XRouter.Processor.IProcessor"/>
    public interface IProcessingProvider : IXRouterComponent
    {
        void Process(Message message);
    }
}
