using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using XRouter.Common;
using XRouter.Data;
using System.Reflection;
using System.IO;
using XRouter.Common.MessageFlowConfig;

namespace XRouter.Manager
{
    /// <summary>
    /// Implements a console server which provides services to the GUI (over a
    /// WCF web service).
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        AddressFilterMode = AddressFilterMode.Any)]
    internal sealed class ConsoleServer : IConsoleServer
    {
        private static readonly string AdapterPluginsDirectory = "AdapterPlugins";
        private static readonly string ActionPluginsDirectory = "ActionPlugins";

        /// <summary>
        /// URI of the ConsoleServer web service which provides the main services.
        /// </summary>
        private string uri = null;

        /// <summary>
        /// URI of the ConsoleServer web service which provides metadata.
        /// </summary>
        private string metadataUri = null;

        /// <summary>
        /// DaemonNT service name of the managed XRouterService.
        /// </summary>
        private string serviceName = null;

        /// <summary>
        /// WCF web service host.
        /// </summary>
        private ServiceHost wcfHost = null;

        /// <summary>
        /// Reference to a service watcher.
        /// </summary>
        private Watcher serviceWatcher = null;

        /// <summary>
        /// Indicates whether this XRouterManager service is ran in the DaemonNT
        /// debug mode.
        /// </summary>
        private bool IsDebugMode = false;

        /// <summary>
        /// XRouter's shared persistentni storage.
        /// </summary>
        private PersistentStorage storage;

        /// <summary>
        /// A tool for scanning the event log of the managed service.
        /// </summary>
        private EventLogReader eventLogReader = null;

        /// <summary>
        /// A tool for scanning the trace log of the managed service.
        /// </summary>
        private TraceLogReader traceLogReader = null;

        /// <summary>
        /// Information for accessing persistent resources.
        /// </summary>
        private StoragesInfo storagesInfo = null;

        public ConsoleServer(
            string serviceName,
            bool isDebugMode,
            string uri,
            string metadataUri,
            StoragesInfo storagesInfo,
            Watcher watcher)
        {
            this.serviceName = serviceName;
            this.IsDebugMode = isDebugMode;
            this.uri = uri;
            this.metadataUri = metadataUri;
            this.storagesInfo = storagesInfo;
            this.serviceWatcher = watcher;
        }

        /// <summary>
        /// Starts the console server in a new thread.
        /// </summary>
        public void Start()
        {
            // init DB storage
            this.storage = new PersistentStorage(this.storagesInfo.DbConnectionString);

            // init log readers
            this.eventLogReader = new EventLogReader(this.storagesInfo.LogsDirectory, this.serviceName);
            this.traceLogReader = new TraceLogReader(this.storagesInfo.LogsDirectory, this.serviceName);

            ObjectConfigurator.Configurator.CustomItemTypes.Add(new TokenSelectionConfigurationItemType());
            ObjectConfigurator.Configurator.CustomItemTypes.Add(new XRouter.Common.Xrm.XrmUriConfigurationItemType());
            ObjectConfigurator.Configurator.CustomItemTypes.Add(new UriConfigurationItemType());
            UpdatePlugins();

            // create WCF service on a new thread
            Exception exception = null;
            Thread wcfHostThread = new Thread(delegate(object data)
            {
                try
                {
                    this.wcfHost = new ServiceHost(this, new Uri(this.uri));

                    // set binding (WebService - SOAP/HTTP)
                    WSHttpBinding binding = new WSHttpBinding();
                    binding.MaxReceivedMessageSize = int.MaxValue;
                    binding.ReaderQuotas = new XmlDictionaryReaderQuotas()
                    {
                        MaxBytesPerRead = int.MaxValue,
                        MaxArrayLength = int.MaxValue,
                        MaxStringContentLength = int.MaxValue
                    };

                    // set endpoint
                    this.wcfHost.AddServiceEndpoint(typeof(IConsoleServer), binding, "ConsoleServer");

                    // set metadata behavior
                    ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                    smb.HttpGetEnabled = true;
                    smb.HttpGetUrl = new Uri(this.metadataUri);
                    this.wcfHost.Description.Behaviors.Add(smb);
                    foreach (var b in this.wcfHost.Description.Behaviors)
                    {
                        if (b is System.ServiceModel.Description.ServiceDebugBehavior)
                        {
                            var sdb = (System.ServiceModel.Description.ServiceDebugBehavior)b;
                            sdb.IncludeExceptionDetailInFaults = true;
                        }
                    }

                    // open connection
                    this.wcfHost.Open();
                }
                catch (Exception e)
                {
                    exception = e;
                }
            });
            wcfHostThread.Start();
            wcfHostThread.Join();

            if (exception != null)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Stops the console server thread.
        /// </summary>
        public void Stop()
        {
            if (this.wcfHost != null)
            {
                this.wcfHost.Close();
            }
        }

        #region IConsoleServer interface

        public string GetXRouterServiceStatus()
        {
            return this.serviceWatcher.ServiceStatus.ToString();
        }

        public void StartXRouterService(int timeout)
        {
            // TODO: possibly use DaemonNT.ServiceCommands.Start() instead as it
            // handles some special cases better

            if (!this.IsDebugMode)
            {
                ServiceController sc = new ServiceController(this.serviceName);
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, timeout));
            }
            else
            {
                // TODO: it is possible to use DaemonNT.ServiceCommands.DebugStart()
                // or start a new process

                TraceLog.Warning("StartXRouterService() is not supported when XRouterManager is in debug mode.");
            }
        }

        public void StopXRouterService(int timeout)
        {
            if (!this.IsDebugMode)
            {
                this.serviceWatcher.DisableServiceAutoStart();
                ServiceController sc = new ServiceController(this.serviceName);
                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, timeout));
            }
            else
            {
                TraceLog.Warning("StopXRouterService() is not supported when XRouterManager is in debug mode.");
            }
        }

        public ApplicationConfiguration GetConfiguration()
        {
            XDocument configXml = storage.GetApplicationConfiguration();
            var result = new ApplicationConfiguration(configXml);
            return result;
        }

        public void ChangeConfiguration(ApplicationConfiguration config)
        {
            this.storage.SaveApplicationConfiguration(config.Content);
        }

        public EventLogEntry[] GetEventLogEntries(DateTime minDate, DateTime maxDate, LogLevelFilters logLevelFilter, int pageSize, int pageNumber)
        {
            return eventLogReader.GetEntries(minDate, maxDate, logLevelFilter, pageSize, pageNumber);
        }

        public TraceLogEntry[] GetTraceLogEntries(DateTime minDate, DateTime maxDate, LogLevelFilters logLevelFilter, int pageSize, int pageNumber)
        {
            return traceLogReader.GetEntries(minDate, maxDate, logLevelFilter, pageSize, pageNumber);
        }

        public Token[] GetTokens(int pageSize, int pageNumber)
        {
            return storage.GetTokens(pageSize, pageNumber);
        }

        public void UpdatePlugins()
        {
            XDocument xConfig = storage.GetApplicationConfiguration();
            // clone the config so that is can be later compared
            XDocument xOldConfig = XDocument.Parse(xConfig.ToString());

            var config = new ApplicationConfiguration(xConfig);

            string binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string adapterPluginsDirectoryFullPath = Path.Combine(binPath, AdapterPluginsDirectory);
            string actionPluginsDirectoryFullPath = Path.Combine(binPath, ActionPluginsDirectory);
            if (!Directory.Exists(adapterPluginsDirectoryFullPath))
            {
                Directory.CreateDirectory(adapterPluginsDirectoryFullPath);
            }
            if (!Directory.Exists(actionPluginsDirectoryFullPath))
            {
                Directory.CreateDirectory(actionPluginsDirectoryFullPath);
            }

            #region Update adapter plugins
            AdapterType[] oldAdapterTypes = config.GetAdapterTypes();
            foreach (AdapterType adapterType in oldAdapterTypes)
            {
                config.RemoveAdapterType(adapterType.Name);
            }
            var adapterPlugins = PluginLoader.FindPlugins<AdapterPluginAttribute>(adapterPluginsDirectoryFullPath);
            foreach (PluginInfo<AdapterPluginAttribute> adapterPlugin in adapterPlugins)
            {
                string assemblyAndClrType = GetTypeAndRelativeAssemblyPath(
                    AdapterPluginsDirectory, adapterPlugin.AssemblyFullPath,
                    adapterPlugin.TypeFullName);
                AdapterType adapterType = new AdapterType(
                    name: adapterPlugin.PluginAttribute.PluginName,
                    assemblyAndClrType: assemblyAndClrType,
                    description: adapterPlugin.PluginAttribute.PluginDescription,
                    clrType: adapterPlugin.PluginType);
                config.AddAdapterType(adapterType);
            }
            #endregion

            #region Update action plugins
            ActionType[] oldActionTypes = config.GetActionTypes();
            foreach (ActionType actionType in oldActionTypes)
            {
                config.RemoveActionType(actionType.Name);
            }

            #region Add built-in actions

            Type sendMessageActionType = typeof(XRouter.Processor.BuiltInActions.SendMessageAction);
            var sendMessageActionInfo = new PluginInfo<ActionPluginAttribute>(sendMessageActionType.Assembly, sendMessageActionType);
            config.AddActionType(new ActionType(
                   name: sendMessageActionInfo.PluginAttribute.PluginName,
                   assemblyAndClrType: String.Join(",", sendMessageActionInfo.TypeFullName,
                   Path.GetFileName(sendMessageActionInfo.AssemblyFullPath)),
                   description: sendMessageActionInfo.PluginAttribute.PluginDescription,
                   clrType: sendMessageActionInfo.PluginType));

            Type xsltTransformActionType = typeof(XRouter.Processor.BuiltInActions.XsltTransformationAction);
            var xsltTransformActionInfo = new PluginInfo<ActionPluginAttribute>(xsltTransformActionType.Assembly, xsltTransformActionType);
            config.AddActionType(new ActionType(
                   name: xsltTransformActionInfo.PluginAttribute.PluginName,
                   assemblyAndClrType: String.Join(",", xsltTransformActionInfo.TypeFullName,
                   Path.GetFileName(xsltTransformActionInfo.AssemblyFullPath)),
                   description: xsltTransformActionInfo.PluginAttribute.PluginDescription,
                   clrType: xsltTransformActionInfo.PluginType));

            #endregion

            var actionPlugins = PluginLoader.FindPlugins<ActionPluginAttribute>(adapterPluginsDirectoryFullPath).ToList();

            foreach (PluginInfo<ActionPluginAttribute> actionPlugin in actionPlugins)
            {
                string assemblyAndClrType = GetTypeAndRelativeAssemblyPath(
                    ActionPluginsDirectory, actionPlugin.AssemblyFullPath,
                    actionPlugin.TypeFullName);
                ActionType actionType = new ActionType(
                    name: actionPlugin.PluginAttribute.PluginName,
                    assemblyAndClrType: assemblyAndClrType,
                    description: actionPlugin.PluginAttribute.PluginDescription,
                    clrType: actionPlugin.PluginType);
                config.AddActionType(actionType);
            }
            #endregion


            if (!CanBeEqual(xOldConfig, config.Content))
            {
                ChangeConfiguration(config);
            }
        }

        private static string GetTypeAndRelativeAssemblyPath(string basePath, string fileAbsPath, string typeFullName)
        {
            string fileName = Path.GetFileName(fileAbsPath);
            string relativeFilePath = Path.Combine(basePath, fileName);
            string assemblyAndClrType = string.Format("{0},{1}", typeFullName, relativeFilePath);
            return assemblyAndClrType;
        }
        #endregion

        private bool CanBeEqual(XDocument xdocument1, XDocument xdocument2)
        {
            byte[] hash1 = GetXDocumentHash(xdocument1);
            byte[] hash2 = GetXDocumentHash(xdocument2);
            return hash1.SequenceEqual(hash2);
        }

        private byte[] GetXDocumentHash(XDocument xdocument)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream))
            {
                xdocument.WriteTo(xmlWriter);
            }
            memoryStream.Position = 0;

            var xfrm = new System.Security.Cryptography.Xml.XmlDsigC14NTransform(false);
            xfrm.LoadInput(memoryStream);
            byte[] result = xfrm.GetDigestedOutput(new System.Security.Cryptography.SHA1Managed());
            return result;
        }
    }
}
