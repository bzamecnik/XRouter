namespace XRouter.Common.ComponentInterfaces
{
    /// <summary>
    /// API of a broker component to be used by a gateway component.
    /// </summary>
    public interface IBrokerServiceForGateway : IBrokerServiceForComponent
    {
        /// <summary>
        /// Passes the token from the gateway to the broker, makes it
        /// persistent and notifies the dispatcher, so that it can pass for
        /// further processing.
        /// </summary>
        /// <param name="token">token to be received by the broker</param>
        void ReceiveToken(Token token);
    }
}
