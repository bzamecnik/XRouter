using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using XRouter.Management;
using XRouter.Scheduler;
using XRouter.Remoting;

namespace XRouter.Gateway.Implementation
{
    class Gateway : IGateway
    {
        private static readonly string BinPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public string Name { get; private set; }

        private IXRouterManager XRouterManager { get; set; }

        private IScheduler Scheduler { get; set; }

        private Dictionary<string, EndpointsPluginService> EndpointsPluginServices { get; set; }

        public Gateway(IXRouterManager xrouterManager, string name)
        {
            XRouterManager = xrouterManager;
            Name = name;
            xrouterManager.ConnectComponent<IGateway>(Name, this);
        }

        public void Initialize()
        {
            var configuration = XRouterManager.GetConfigData(string.Format("/xrouter/components/gateway[@name=\"{0}\"]", Name)).XElement;

            string targetSchedulerName = configuration.Attribute(XName.Get("targetScheduler")).Value;
            Scheduler = XRouterManager.GetComponent<IScheduler>(targetSchedulerName);

            EndpointsPluginServices = new Dictionary<string, EndpointsPluginService>();
            var pluginsConfig = configuration.Element("endpointsPlugins").Elements("endpointsPlugin");
            foreach (var pluginConfig in pluginsConfig) {
                string pluginName = pluginConfig.Attribute(XName.Get("name")).Value;
                string typeAddress = pluginConfig.Attribute(XName.Get("type")).Value;
                var pluginService = GetEndpointsPluginInstance(typeAddress, pluginConfig, pluginName);
                EndpointsPluginServices.Add(pluginName, pluginService);
            }

            Start();
        }

        private EndpointsPluginService GetEndpointsPluginInstance(string typeAddress, XElement pluginConfig, string pluginName)
        {
            string[] addressParts = typeAddress.Split(';');
            string assemblyFile = addressParts[0].Trim();
            string typeFullName = addressParts[1].Trim();

            string assemblyFullPath = Path.Combine(BinPath, assemblyFile);
            Assembly assembly = Assembly.LoadFile(assemblyFullPath);
            Type type = assembly.GetType(typeFullName, true);

            var pluginService = new EndpointsPluginService(this, pluginName);
            var constructor = type.GetConstructor(new Type[] { typeof(XElement), typeof(IEndpointsPluginService) });
            var pluginObject = constructor.Invoke(new object[] { pluginConfig, pluginService });
            var plugin = (IEndpointsPlugin)pluginObject;
            pluginService.Client = plugin;
            return pluginService;
        }

        public void Start()
        {
            #region Register endpoints
            foreach (var pluginService in EndpointsPluginServices.Values) {
                
                foreach (var inputEndPoint in pluginService.InputEndpoints) {
                    inputEndPoint.MessageReceived += DispatchInputMessage;
                }

                foreach (var outputEndPoint in pluginService.OutputEndpoints) {
                    XRouterManager.RegisterOutputEndpoint(outputEndPoint);
                }
            }
            #endregion

            #region Start endpoints
            foreach (var pluginService in EndpointsPluginServices.Values) {
                pluginService.Client.Start();
            }
            #endregion            
        }

        public void Stop()
        {
            #region Stop endpoints
            foreach (var pluginService in EndpointsPluginServices.Values) {
                pluginService.Client.Stop();
            }
            #endregion

            #region Unregister endpoints
            foreach (var plugin in EndpointsPluginServices.Values) {

                foreach (var inputEndPoint in plugin.InputEndpoints) {                    
                    inputEndPoint.MessageReceived -= DispatchInputMessage;
                }

                foreach (var outputEndPoint in plugin.OutputEndpoints) {
                    XRouterManager.UnregisterOutputEndpoint(outputEndPoint);
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
