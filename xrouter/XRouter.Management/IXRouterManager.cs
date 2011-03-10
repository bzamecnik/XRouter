using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Remoting;

namespace XRouter.Management
{
    public delegate void ConfigChangedHandler(RemotableXElement changeRoot);

    public interface IXRouterManager : IRemotelyReferable
    {
        #region Managing components (registration and lookup)
        void ConnectComponent<T>(string name, T component) where T : IXRouterComponent;
        T GetComponent<T>(string name) where T : IXRouterComponent;
        #endregion

        #region Configuration services
        RemotableXElement GetConfigData(string xpath);
        event ConfigChangedHandler ConfigChanged;
        #endregion

        #region Managing output endpoints (Gateway adapters can register them and MessageProcessors access them)
        void RegisterOutputEndpoint(IOutputEndpoint endpoint);
        void UnregisterOutputEndpoint(IOutputEndpoint endpoint);
        IOutputEndpoint GetOutputEndpoint(EndpointAddress endpointAddress);
        IEnumerable<IOutputEndpoint> GetOutputEndpoints();
        #endregion

        #region Storing message token
        void StoreMessageToken(Message message); // Message token can be a fresh new message token straight from Gateway or a message token being processed in a message processor
        #endregion

        #region Error events
        void StoreErrorEvent(ErrorEvent errorEvent);
        IEnumerable<ErrorEvent> GetErrorEvents();
        #endregion

        #region General logging
        void Log(IXRouterComponent component, string category, string message);
        #endregion
    }
}
