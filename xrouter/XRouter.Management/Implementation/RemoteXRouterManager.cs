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

        public RemoteXRouterManager(string[] arguments)
        {
            string managerUrl = arguments[0];

        }
        public RemotableXElement GetConfigData(string xpath)
        {
            throw new NotImplementedException();
        }

        public void ConnectComponent<T>(string name, T component) where T : IXRouterComponent
        {
            throw new NotImplementedException();
        }

        public T GetComponent<T>(string name) where T : IXRouterComponent
        {
            throw new NotImplementedException();
        }

        public void RegisterEndpoint(IEndpoint endpoint)
        {
            throw new NotImplementedException();
        }

        public void UnregisterEndpoint(IEndpoint endpoint)
        {
            throw new NotImplementedException();
        }

        public IInputEndpoint GetInputEndpoint(EndpointAddress endpointAddress)
        {
            throw new NotImplementedException();
        }

        public IOutputEndpoint GetOutputEndpoint(EndpointAddress endpointAddress)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IInputEndpoint> GetInputEndpoints()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IOutputEndpoint> GetOutputEndpoints()
        {
            throw new NotImplementedException();
        }

        public void Log(string category, string message)
        {
            throw new NotImplementedException();
        }
    }
}
