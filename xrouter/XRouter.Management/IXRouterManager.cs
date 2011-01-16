using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XRouter.Management
{
    public delegate void ConfigChangeHandler(XElement changeRoot);

    public interface IXRouterManager
    {
        event ConfigChangeHandler ConfigChanged;

        XElement GetConfigData(string xpath);

        void ConnectComponent<T>(string name, T component)
            where T : IXRouterComponent;

        T GetComponent<T>(string name)
            where T : IXRouterComponent;

        void RegisterEndpoint(Endpoint endpoint);
        void UnregisterEndpoint(Endpoint endpoint);
        InputEndpoint GetInputEndpoint(EndpointAddress endpointAddress);
        OutputEndpoint GetOutputEndpoint(EndpointAddress endpointAddress);
        IEnumerable<InputEndpoint> GetInputEndpoints();
        IEnumerable<OutputEndpoint> GetOutputEndpoints();

        void Log(string category, string message);
    }
}
