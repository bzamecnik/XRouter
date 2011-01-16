using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XRouter.Management.Implementation
{
    class XRouterManager : IXRouterManager
    {
        private static readonly string BinPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private readonly string ConfigFilePath = Path.Combine(BinPath, @"..\..\..\XRouter.Management\Data files\xrouter-config.xml");
        private readonly string LogDirectory = Path.Combine(BinPath, @"..\..\..\XRouter.Management\Data files\Logs");
        private readonly string LogFileNameFormat = "log-{0}.txt";

        public event ConfigChangeHandler ConfigChanged;

        private Dictionary<string, IXRouterComponent> connectedComponents = new Dictionary<string, IXRouterComponent>();

        private Dictionary<EndpointAddress, InputEndpoint> inputEndpoints = new Dictionary<EndpointAddress, InputEndpoint>();
        private Dictionary<EndpointAddress, OutputEndpoint> outputEndpoints = new Dictionary<EndpointAddress, OutputEndpoint>();

        public XRouterManager()
        {
        }

        public void ConnectComponent<T>(string name, T component)
            where T : IXRouterComponent
        {
            connectedComponents.Add(name, component);

            bool areAllComponentsConnected = true;
            var componentsConfig = GetConfigData("/xrouter/components").Elements();
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

        public void RegisterEndpoint(Endpoint endpoint)
        {
            if (endpoint is InputEndpoint) {
                inputEndpoints.Add(endpoint.Address, (InputEndpoint)endpoint);
            } else {
                outputEndpoints.Add(endpoint.Address, (OutputEndpoint)endpoint);
            }
        }

        public void UnregisterEndpoint(Endpoint endpoint)
        {
            if (endpoint is InputEndpoint) {
                inputEndpoints.Remove(endpoint.Address);
            } else {
                outputEndpoints.Remove(endpoint.Address);
            }
        }

        public InputEndpoint GetInputEndpoint(EndpointAddress endpointAddress)
        {
            var result = inputEndpoints[endpointAddress];
            return result;
        }

        public OutputEndpoint GetOutputEndpoint(EndpointAddress endpointAddress)
        {
            var result = outputEndpoints[endpointAddress];
            return result;
        }

        public IEnumerable<InputEndpoint> GetInputEndpoints()
        {
            return inputEndpoints.Values;
        }

        public IEnumerable<OutputEndpoint> GetOutputEndpoints()
        {
            return outputEndpoints.Values;
        }

        public XElement GetConfigData(string xpath)
        {
            if (!File.Exists(ConfigFilePath)) {
                return null;
            }

            var xdoc = XDocument.Load(ConfigFilePath);
            var result = xdoc.XPathSelectElement(xpath);
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
