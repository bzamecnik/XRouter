using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaemonNT.Logging;
using System.Threading;
using System.ServiceProcess;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Xml;
using System.Xml.Linq;
using XRouter.Common;
using XRouter.Common.Persistence;

namespace XRouter.Manager
{
    /// <summary>
    /// Specfikace rozhrani WCF, ktera poskytuje sluzby XRouter console (GUI).
    /// </summary>
    [ServiceContract]
    public interface IConsoleServer
    {
        [OperationContract]
        string GetXRouterServiceStatus();
       
        [OperationContract]
        void StartXRouterService(int timeout);
        
        [OperationContract]
        void StopXRouterService(int timeout);

        [OperationContract]
        ApplicationConfiguration GetConfiguration();

        [OperationContract]
        void ChangeConfiguration(ApplicationConfiguration config);

        [OperationContract]
        EventLogEntry[] GetEventLogEntries(DateTime minDate, DateTime maxDate, LogLevelFilters logLevelFilter, int pageSize, int pageNumber);

        [OperationContract]
        TraceLogEntry[] GetTraceLogEntries(DateTime minDate, DateTime maxDate, LogLevelFilters logLevelFilter, int pageSize, int pageNumber);

        [OperationContract]
        Token[] GetTokens(int pageSize, int pageNumber);
    }

    /// <summary>
    /// Implementace WCF sluzby, ktera poskytuje sluzby XRouter console (GUI). 
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    internal sealed class ConsoleServer : IConsoleServer
    {
        /// <summary>
        /// Uri tohoto Web Serveru.
        /// </summary>
        private string uri = null;

        /// <summary>
        /// Uri tohoto Web Serveru, na kterem poskytuje metadata. 
        /// </summary>
        private string metadataUri = null;

        /// <summary>
        /// ServiceName instance XRouterService, se kterou je tato sluzba asociovana.
        /// </summary>
        private string serviceName = null;

        /// <summary>
        /// Hostovaci prostredi pro tuto WCF Web Service. 
        /// </summary>
        private ServiceHost wcfHost = null;
        
        /// <summary>
        /// Odkaz na DaemonNT trace logger.
        /// </summary>
        private TraceLogger logger = null;

        /// <summary>
        /// Odkaz na service watcher.
        /// </summary>
        private Watcher serviceWatcher = null;

        /// <summary>
        /// Urcuje, jestli je instance teto sluzby spustena v Daemon debug modu. 
        /// </summary>
        private bool IsDebugMode = false;

        /// <summary>
        /// Persistentni uloziste XRouter. 
        /// </summary>
        private PersistentStorage storage;
        
        /// <summary>
        /// Nastroj pro skenovani event logu.
        /// </summary>
        private EventLogReader eventLogReader = null;

        /// <summary>
        /// Nastroj pro skenovani trace logu. 
        /// </summary>
        private TraceLogReader traceLogReader = null;

        /// <summary>
        /// Informace pro pristup k persistentnim zdrojum. 
        /// </summary>
        private StoragesInfo storagesInfo = null;

        public ConsoleServer(string serviceName, bool isDebugMode, string uri, string metadataUri, 
            StoragesInfo storagesInfo, Watcher watcher, TraceLogger logger)
        {
            this.serviceName = serviceName;
            this.IsDebugMode = isDebugMode;
            this.uri = uri;           
            this.metadataUri = metadataUri;
            this.storagesInfo = storagesInfo;
            this.serviceWatcher = watcher;
            this.logger = logger;                      
        }
      
        public void Start()
        {
            // init DB storage
            this.storage = new PersistentStorage(this.storagesInfo.DbConnectionString);

            // init log readers
            this.eventLogReader = new EventLogReader(this.storagesInfo.LogsDirectory);
            this.traceLogReader = new TraceLogReader(this.storagesInfo.LogsDirectory);
            
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
                    binding.ReaderQuotas = new XmlDictionaryReaderQuotas() { MaxBytesPerRead = int.MaxValue, 
                        MaxArrayLength = int.MaxValue, MaxStringContentLength = int.MaxValue };                    
                               
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
        
        public void Stop()
        {
            if (this.wcfHost != null)
            {
                this.wcfHost.Close();
            }
        }

        /// <summary>
        /// Vraci status sluzby XRouterService (running, stopped, ...). 
        /// Pokud je metoda volana v Daemon debug modu, pak vraci pouze stav Stopped.
        /// </summary>
        /// <returns></returns>
        public string GetXRouterServiceStatus()
        {
            return this.serviceWatcher.ServiceStatus.ToString();
        }

        /// <summary>
        /// Spusti XRouterService. Metoda ceka na to, nez XRouterService prejde do stavu running.
        /// Pokud behem daneho timeoutu neprejde sluzba do stavu running, pak je vyhozena vyjimka.
        /// Metoda nema zadny efekt v Daemon debug modu.
        /// </summary>
        /// <param name="timeout"></param>
        public void StartXRouterService(int timeout)
        {
            if (!this.IsDebugMode)
            {
                ServiceController sc = new ServiceController(this.serviceName);
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, timeout));
            }
            else
            {
                this.logger.LogWarning("StartXRouterService() is not allowed on debug mode.");
            }
        }

        /// <summary>
        /// Zastavi XRouterService. Metoda ceka na to, nez XRouterService prejde do stavu stopped.
        /// Pokud behem daneho timeoutu neprejde sluzba do stavu stopped, pak je vyhozena vyjimka.
        /// Metoda nema zadny efekt v Daemon debug modu. 
        /// </summary>
        /// <param name="timeout">Urcuje dobu cekani v sec na to, nez XRouterService prejde do stavu stopped.</param>
        public void StopXRouterService(int timeout)
        {
            if (!this.IsDebugMode)
            {
                this.serviceWatcher.DisableServiceThrowUp();
                ServiceController sc = new ServiceController(this.serviceName);
                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, timeout));
            }
            else
            {
                this.logger.LogWarning("StopXRouterService() is not allowed on debug mode.");
            }
        }

        /// <summary>
        /// Vraci konfiguraci z persistentniho uloziste.
        /// </summary>
        /// <returns></returns>
        public ApplicationConfiguration GetConfiguration()
        {
            XDocument configXml = storage.GetApplicationConfiguration();
            var result = new ApplicationConfiguration(configXml);
            return result;            
        }

        /// <summary>
        /// Aktualizuje konfiguraci v persistentnim ulozisti. 
        /// </summary>
        /// <param name="config"></param>
        public void ChangeConfiguration(ApplicationConfiguration config)
        {
            this.storage.SaveApplicationConfiguration(config.Content);
        }

        /// <summary>
        /// Vybere a vrati event log zaznamy dle danych kriterii. 
        /// </summary>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        /// <param name="logLevelFilter"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public EventLogEntry[] GetEventLogEntries(DateTime minDate, DateTime maxDate, LogLevelFilters logLevelFilter, int pageSize, int pageNumber)
        {
            return eventLogReader.GetEntries(minDate, maxDate, logLevelFilter, pageSize, pageNumber);
        }

        /// <summary>
        /// Vybere a vrati trace log zaznamy dle danych kriterii. 
        /// </summary>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        /// <param name="logLevelFilter"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public TraceLogEntry[] GetTraceLogEntries(DateTime minDate, DateTime maxDate, LogLevelFilters logLevelFilter, int pageSize, int pageNumber)
        {
            return traceLogReader.GetEntries(minDate, maxDate, logLevelFilter, pageSize, pageNumber);
        }

        /// <summary>
        /// Vybere a vrati tokeny dle danych kriterii. 
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public Token[] GetTokens(int pageSize, int pageNumber)
        {
            return storage.GetTokens(pageSize, pageNumber);
        }
    }
}
