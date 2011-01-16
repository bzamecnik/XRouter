using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using XRouter.Management;
using XRouter.Scheduler;

namespace XRouter.Gateway.Implementation
{
    class GatewayService : IGateway
    {
        private static readonly string BinPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public string Name { get; private set; }

        private IXRouterManager XRouterManager { get; set; }

        private IScheduler Scheduler { get; set; }

        private Dictionary<string, IEndpointsPlugin> EndpointsPlugins { get; set; }        

        public GatewayService(IXRouterManager xrouterManager, string name)
        {
            XRouterManager = xrouterManager;
            Name = name;
            xrouterManager.ConnectComponent(Name, this);
        }

        public void Initialize()
        {
            var configuration = XRouterManager.GetConfigData(string.Format("/xrouter/components/gateway[@name=\"{0}\"]", Name));

            string targetSchedulerName = configuration.Attribute(XName.Get("targetScheduler")).Value;
            Scheduler = XRouterManager.GetComponent<IScheduler>(targetSchedulerName);

            EndpointsPlugins = new Dictionary<string, IEndpointsPlugin>();
            var pluginsConfig = configuration.Element("endpointsPlugins").Elements("endpointsPlugin");
            foreach (var pluginConfig in pluginsConfig) {
                string pluginName = pluginConfig.Attribute(XName.Get("name")).Value;
                string typeAddress = pluginConfig.Attribute(XName.Get("type")).Value;
                var plugin = GetEndpointsPluginInstance(typeAddress, pluginConfig);
                EndpointsPlugins.Add(pluginName, plugin);
            }

            Start();
        }

        private IEndpointsPlugin GetEndpointsPluginInstance(string typeAddress, XElement pluginConfig)
        {
            string[] addressParts = typeAddress.Split(';');
            string assemblyFile = addressParts[0].Trim();
            string typeFullName = addressParts[1].Trim();

            string assemblyFullPath = Path.Combine(BinPath, assemblyFile);
            Assembly assembly = Assembly.LoadFile(assemblyFullPath);
            Type type = assembly.GetType(typeFullName, true);

            var constructor = type.GetConstructor(new Type[] { typeof(XElement), typeof(IGateway) });
            var pluginObject = constructor.Invoke(new object[] { pluginConfig, this });
            var result = (IEndpointsPlugin)pluginObject;
            return result;
        }

        public void Start()
        {
            #region Register endpoints
            foreach (var plugin in EndpointsPlugins.Values) {
                
                foreach (var inputEndPoint in plugin.InputEndpoints) {                    
                    XRouterManager.RegisterEndpoint(inputEndPoint);
                    inputEndPoint.MessageReceived += DispatchInputMessage;
                }

                foreach (var outputEndPoint in plugin.OutputEndpoints) {
                    XRouterManager.RegisterEndpoint(outputEndPoint);
                }
            }
            #endregion

            #region Start endpoints
            foreach (var plugin in EndpointsPlugins.Values) {
                plugin.Start();
            }
            #endregion            
        }

        public void Stop()
        {
            #region Stop endpoints
            foreach (var plugin in EndpointsPlugins.Values) {
                plugin.Stop();
            }
            #endregion

            #region Unregister endpoints
            foreach (var plugin in EndpointsPlugins.Values) {

                foreach (var inputEndPoint in plugin.InputEndpoints) {                    
                    XRouterManager.UnregisterEndpoint(inputEndPoint);
                    inputEndPoint.MessageReceived -= DispatchInputMessage;
                }

                foreach (var outputEndPoint in plugin.OutputEndpoints) {
                    XRouterManager.UnregisterEndpoint(outputEndPoint);
                }
            }
            #endregion
        }

        private void DispatchInputMessage(Message message)
        {
            Scheduler.ScheduleMessage(message);
        }
    }
}
