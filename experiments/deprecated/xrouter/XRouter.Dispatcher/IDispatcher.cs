using XRouter.Management;

namespace XRouter.Dispatcher
{
    /// <summary>
    /// Message Dispatcher.
    /// 
    /// It receives messages from Gateways and dispatches them to Message
    /// Processors for the actual processing.
    /// 
    /// The purpose
    /// </summary>
    /// <seealso cref="XRouter.Processor.IProcessor"/>
    /// <seealso cref="XRouter.Gateway.IGateway"/>
    public interface IDispatcher : IXRouterComponent
    {
        void Dispatch(Message message);
    }
}
