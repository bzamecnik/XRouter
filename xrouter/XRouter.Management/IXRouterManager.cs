using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Remoting;

namespace XRouter.Management
{
    public delegate void ConfigChangedHandler(RemotableXElement changeRoot);

    public interface IXRouterManager
    {
        RemotableXElement GetConfigData(string xpath);

        event ConfigChangedHandler ConfigChanged;

        void ConnectComponent<T>(string name, T component)
            where T : IXRouterComponent;

        T GetComponent<T>(string name)
            where T : IXRouterComponent;

        void RegisterEndpoint(IEndpoint endpoint);
        void UnregisterEndpoint(IEndpoint endpoint);
        IInputEndpoint GetInputEndpoint(EndpointAddress endpointAddress);
        IOutputEndpoint GetOutputEndpoint(EndpointAddress endpointAddress);
        IEnumerable<IInputEndpoint> GetInputEndpoints();
        IEnumerable<IOutputEndpoint> GetOutputEndpoints();

        void Log(string category, string message);
    }
}
