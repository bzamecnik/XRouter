using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Remoting;

namespace XRouter.Management.Implementation
{
    class XRouterManagerProxy : IXRouterManager
    {
        public event ConfigChangedHandler ConfigChanged = delegate { };

        private IXRouterManager managerProxy;

        public XRouterManagerProxy(string[] arguments)
        {
            var managerAddress = RemoteObjectAddress.Deserialize(arguments[0]);
            managerProxy = RemoteObjectProxyProvider.GetProxy<IXRouterManager>(managerAddress);
            managerProxy.ConfigChanged += new ConfigChangedHandler(managerProxy_ConfigChanged);
        }

        public void ConnectComponent<T>(string name, T component)
            where T : IXRouterComponent
        {
            managerProxy.ConnectComponent(name, component);
        }

        public T GetComponent<T>(string name)
            where T : IXRouterComponent
        {
            return managerProxy.GetComponent<T>(name);
        }

        public RemotableXElement GetConfigData(string xpath)
        {
            return managerProxy.GetConfigData(xpath);
        }

        void managerProxy_ConfigChanged(RemotableXElement changeRoot)
        {
            ConfigChanged(changeRoot);
        }

        public void RegisterOutputEndpoint(IOutputEndpoint endpoint)
        {
            managerProxy.RegisterOutputEndpoint(endpoint);
        }

        public void UnregisterOutputEndpoint(IOutputEndpoint endpoint)
        {
            managerProxy.UnregisterOutputEndpoint(endpoint);
        }

        public IOutputEndpoint GetOutputEndpoint(EndpointAddress endpointAddress)
        {
            return managerProxy.GetOutputEndpoint(endpointAddress);
        }

        public IEnumerable<IOutputEndpoint> GetOutputEndpoints()
        {
            return managerProxy.GetOutputEndpoints();
        }

        public void StoreMessageToken(Message message)
        {
            managerProxy.StoreMessageToken(message);
        }

        public void StoreErrorEvent(ErrorEvent errorEvent)
        {
            managerProxy.StoreErrorEvent(errorEvent);
        }

        public IEnumerable<ErrorEvent> GetErrorEvents()
        {
            return managerProxy.GetErrorEvents();
        }

        public void Log(IXRouterComponent component, string category, string message)
        {
            managerProxy.Log(component, category, message);
        }
    }
}
