using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using XRouter.Remoting;

namespace XRouter.Management.Implementation
{
    class LocalXRouterManager : IXRouterManager
    {
        private static readonly string BinPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private readonly string ConfigFilePath = Path.Combine(BinPath, @"..\..\..\XRouter.Management\Data files\xrouter-config.xml");
        private readonly string LogDirectory = Path.Combine(BinPath, @"..\..\..\XRouter.Management\Data files\Logs");
        private readonly string LogFileNameFormat = "log-{0}.txt";

        public event ConfigChangedHandler ConfigChanged = delegate { };

        private Dictionary<string, IXRouterComponent> connectedComponents = new Dictionary<string, IXRouterComponent>();

        private Dictionary<EndpointAddress, IInputEndpoint> inputEndpoints = new Dictionary<EndpointAddress, IInputEndpoint>();
        private Dictionary<EndpointAddress, IOutputEndpoint> outputEndpoints = new Dictionary<EndpointAddress, IOutputEndpoint>();

        public LocalXRouterManager()
        {
        }

        public void ConnectComponent<T>(string name, T component)
            where T : IXRouterComponent
        {
            connectedComponents.Add(name, component);

            bool areAllComponentsConnected = true;
            var componentsConfig = GetConfigData("/xrouter/components").XElement.Elements();
            foreach (var componentConfig in componentsConfig) {
                string componentName = componentConfig.Attribute(XName.Get("name")).Value;
                if (!connectedComponents.ContainsKey(componentName)) {
                    areAllComponentsConnected = false;
                    break;
                }
            }

            if (areAllComponentsConnected) {
                foreach (var c in connectedComponents.Values) {
                    c.Initialize();
                }
            }
        }

        public T GetComponent<T>(string name)
            where T : IXRouterComponent
        {
            var component = connectedComponents[name];
            var result = (T)component;
            return result;
        }

        public void RegisterEndpoint(IEndpoint endpoint)
        {
            if (endpoint is IInputEndpoint) {
                inputEndpoints.Add(endpoint.Address, (IInputEndpoint)endpoint);
            } else {
                outputEndpoints.Add(endpoint.Address, (IOutputEndpoint)endpoint);
            }
        }

        public void UnregisterEndpoint(IEndpoint endpoint)
        {
            if (endpoint is IInputEndpoint) {
                inputEndpoints.Remove(endpoint.Address);
            } else {
                outputEndpoints.Remove(endpoint.Address);
            }
        }

        public IInputEndpoint GetInputEndpoint(EndpointAddress endpointAddress)
        {
            var result = inputEndpoints[endpointAddress];
            return result;
        }

        public IOutputEndpoint GetOutputEndpoint(EndpointAddress endpointAddress)
        {
            var result = outputEndpoints[endpointAddress];
            return result;
        }

        public IEnumerable<IInputEndpoint> GetInputEndpoints()
        {
            return inputEndpoints.Values;
        }

        public IEnumerable<IOutputEndpoint> GetOutputEndpoints()
        {
            return outputEndpoints.Values;
        }

        public RemotableXElement GetConfigData(string xpath)
        {
            if (!File.Exists(ConfigFilePath)) {
                return null;
            }

            var xdoc = XDocument.Load(ConfigFilePath);
            var element = xdoc.XPathSelectElement(xpath);
            var result = new RemotableXElement(element);
            return result;
        }

        public void Log(string category, string message)
        {
            if (!Directory.Exists(LogDirectory)) {
                Directory.CreateDirectory(LogDirectory);
            }

            string logFilePath = Path.Combine(LogDirectory, string.Format(LogFileNameFormat, category));
            string logLine = string.Format("{0} {1} {2}", DateTime.Now, message, Environment.NewLine);
            File.AppendAllText(logFilePath, logLine);
        }
    }
}
