using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Remoting;

namespace XRouter.Management.Implementation
{
    class RemoteXRouterManager : IXRouterManager
    {
        public event ConfigChangedHandler ConfigChanged = delegate { };

        private IXRouterManager managerProxy;

        public RemoteXRouterManager(string[] arguments)
        {
            var managerAddress = RemoteObjectAddress.Deserialize(arguments[0]);
            managerProxy = RemoteObjectProxyProvider.GetProxy<IXRouterManager>(managerAddress);
            managerProxy.ConfigChanged += new ConfigChangedHandler(managerProxy_ConfigChanged);
        }

        void managerProxy_ConfigChanged(RemotableXElement changeRoot)
        {
            ConfigChanged(changeRoot);
        }

        public RemotableXElement GetConfigData(string xpath)
        {
            return managerProxy.GetConfigData(xpath);
        }

        public void ConnectComponent<T>(string name, T component) where T : IXRouterComponent
        {
            managerProxy.ConnectComponent<T>(name, component);
        }

        public T GetComponent<T>(string name) where T : IXRouterComponent
        {
            return managerProxy.GetComponent<T>(name);
        }

        public void RegisterEndpoint(IEndpoint endpoint)
        {
            managerProxy.RegisterEndpoint(endpoint);
        }

        public void UnregisterEndpoint(IEndpoint endpoint)
        {
            managerProxy.UnregisterEndpoint(endpoint);
        }

        public IInputEndpoint GetInputEndpoint(EndpointAddress endpointAddress)
        {
            return managerProxy.GetInputEndpoint(endpointAddress);
        }

        public IOutputEndpoint GetOutputEndpoint(EndpointAddress endpointAddress)
        {
            return managerProxy.GetOutputEndpoint(endpointAddress);
        }

        public IEnumerable<IInputEndpoint> GetInputEndpoints()
        {
            return managerProxy.GetInputEndpoints();
        }

        public IEnumerable<IOutputEndpoint> GetOutputEndpoints()
        {
            return managerProxy.GetOutputEndpoints();
        }

        public void Log(string category, string message)
        {
            managerProxy.Log(category, message);
        }
    }
}
