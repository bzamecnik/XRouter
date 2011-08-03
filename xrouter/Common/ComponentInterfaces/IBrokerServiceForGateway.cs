
namespace XRouter.Common.ComponentInterfaces
{
    // methods to be called by a gateway
    public interface IBrokerServiceForGateway : IBrokerServiceForComponent
    {
        // - pass token from gateway to broker
        // - notify dispatcher
        void ReceiveToken(Token token);
    }
}
