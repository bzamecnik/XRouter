namespace DaemonNT.Logging
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Collections.Generic;
    using DaemonNT.Configuration;

    /// <summary>
    /// Poskytuje efektivni, vice-vlaknove logovani potencialne low-level informaci.
    /// </summary>
    public sealed class TraceLogger
    {        
        private LoggerImplementation loggerImpl = null;

        private ILoggerStorage[] storages = null;

        private TraceLogger()
        { }

        /// <summary>
        /// Vytvoří a inicializuje logger. Po zavolání metody je možno začít logovat. 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="bufferSize"></param>
        /// <param name="isDebugMode"></param>
        /// <param name="settings"></param>
        /// <param name="eventLogger"></param>
        /// <returns></returns>
        internal static TraceLogger Create(string serviceName, int bufferSize, bool isDebugMode, 
            TraceLoggerSettings settings, EventLogger eventLogger)
        {
            TraceLogger instance = new TraceLogger();

            List<ILoggerStorage> storages = new List<ILoggerStorage>();
            
            // add standard storage
            storages.Add(new LoggerFileStorage(string.Format("{0}.Trace", serviceName)));
            
            // add configured storages
            foreach (TraceLoggerStorageSettings traceLoggerSettings in settings.Storages)
            {
                // create and initialize storage
                TraceLoggerStorage storage = TypesProvider.CreateTraceLoggerStorage(traceLoggerSettings.TypeClass, traceLoggerSettings.TypeAssembly);
                storage.Event = eventLogger; 
               
                // create adapter and start storage
                TraceLoggerStorageAdapter storageWrapper = new TraceLoggerStorageAdapter(storage);
                storageWrapper.Start(traceLoggerSettings.Name, isDebugMode, traceLoggerSettings.Settings);

                storages.Add(storageWrapper);
            }

            instance.storages = storages.ToArray();
            instance.loggerImpl = LoggerImplementation.Start(instance.storages, bufferSize);

            return instance;
        }
       
        /// <summary>
        /// Ukončí práci loggeru (zastaví příjem logů, vyprázdní buffer apod.)
        /// </summary>
        /// <param name="shutdown">
        /// Určuje, jestli došlo k vypnutí loggeru manuálně nebo při vypnutí instance
        /// operačního systému. 
        /// </param>
        internal void Close(bool shutdown)
        {
            // ukončí příjem logů a počká na vyprázdnění bufferu
            this.loggerImpl.Stop();

            // notifikuje nakonfigurované trace-logger-storages a zastavení
            foreach (ILoggerStorage loggerStorage in this.storages)
            {
                TraceLoggerStorageAdapter adapter = loggerStorage as TraceLoggerStorageAdapter;

                if (adapter != null)
                {                    
                    adapter.Stop(shutdown);
                }
            }
        }

        /// <summary>
        /// Vytváří a vkládá trace-log do vnitřního bufferu, ze kterého je následně přesunut do 
        /// persistentního úložiště.
        /// </summary>
        /// <param name="logType">
        /// Specifikace typu logu. 
        /// </param>
        /// <param name="xmlContent">
        /// Libovolný XML content, pomocí které je možno dosáhnout přizpůsobení problémové 
        /// doméně a zaznamenávat strukturované informace.
        /// </param>
        public void Log(LogType logType, string xmlContent)
        {
            TraceLog log = TraceLog.Create(logType, xmlContent);
            this.loggerImpl.PushLog(log);  
        }

        /// <summary>
        /// Vytváří a vkládá info trace-log do vnitřního bufferu, ze kterého je následně přesunut do 
        /// persistentního úložiště.
        /// </summary>
        /// <param name="content">
        /// Libovolný XML content, pomocí které je možno dosáhnout přizpůsobení problémové 
        /// doméně a zaznamenávat strukturované informace.
        /// </param>
        /// <remarks>
        /// Metoda je thread-safe. 
        /// </remarks>
        public void LogInfo(string xmlContent)
        {
            this.Log(LogType.Info, xmlContent);          
        }

        /// <summary>
        /// Vytváří a vkládá warning trace-log do vnitřního bufferu, ze kterého je následně přesunut do 
        /// persistentního úložiště.
        /// </summary>
        /// <param name="xmlContent">
        /// Libovolný XML content, pomocí které je možno dosáhnout přizpůsobení problémové 
        /// doméně a zaznamenávat strukturované informace.
        /// </param>
        public void LogWarning(string xmlContent)
        {
            this.Log(LogType.Warning, xmlContent);
        }

        /// <summary>
        /// Vytváří a vkládá error trace-log do vnitřního bufferu, ze kterého je následně přesunut do 
        /// persistentního úložiště.
        /// </summary>
        /// <param name="xmlContent">
        /// Libovolný XML content, pomocí které je možno dosáhnout přizpůsobení problémové 
        /// doméně a zaznamenávat strukturované informace.
        /// </param>
        public void LogError(string xmlContent)
        {
            this.Log(LogType.Error, xmlContent);
        }

        /// <summary>
        /// Vytváří a vkládá error trace-log obsahující serializovanou Exception do vnitřního bufferu, ze 
        /// kterého je následně přesunut do persistentního úložiště.
        /// </summary>
        /// <param name="e">Exception, která se má serializovat.</param>
        public void LogException(Exception e)
        {                     
            this.Log(LogType.Error, SerializeException(e));
        }

        /// <summary>
        /// Serializuje danou výjimku a všechny vnitřní výjimky do XML contentu.  
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private String SerializeException(Exception e)
        {                                          
            StringBuilder sb = new StringBuilder();
            
            // type
            sb.Append(String.Format("<exception type=\"{0}\">", e.GetType().ToString()));

            // message
            if (e.Message != null)
            {
                sb.Append(String.Format("<message>{0}</message>", e.Message));
            }
           
            // data
            if (e.Data.Count > 0)
            {
                sb.Append("<data>");
                foreach (System.Collections.DictionaryEntry entry in e.Data)
                {
                    String key = entry.Key.ToString();
                    String value = entry.Value.ToString();
                    sb.Append(String.Format("<entry key=\"{0}\" value=\"{1}\"/>", key, value));
                }
                sb.Append("</data>");
            }

            // stack-trace
            sb.Append(String.Format("<stack-trace>{0}</stack-trace>", e.StackTrace));

            // inner exception
            if (e.InnerException != null)
            {
                sb.Append(String.Format("{0}", SerializeException(e.InnerException)));               
            }

            sb.Append(String.Format("</exception>"));
            
            return sb.ToString();
        }
    }
}
