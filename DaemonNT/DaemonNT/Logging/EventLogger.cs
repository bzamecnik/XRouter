namespace DaemonNT.Logging
{
    using System;
    using System.Collections.Generic;
          
    /// <summary>
    /// Provides a facility for efficient, thread-safe logging of
    /// important events designated for service administrators.
    /// </summary>
    public class EventLogger
    {
        private LoggerImplementation loggerImpl = null;
       
        private EventLogger()
        { }

        internal static EventLogger Create(String serviceName, Int32 bufferSize)
        {
            EventLogger instance = new EventLogger(); 
                      
            // create storages
            List<ILoggerStorage> storages = new List<ILoggerStorage>();
            storages.Add(new LoggerFileStorage(serviceName));

            // start logger
            instance.loggerImpl = LoggerImplementation.Start(storages.ToArray(), bufferSize);

            return instance;
        }
      
        internal void Close()
        {
            this.loggerImpl.Stop();
        }
 
        public void Log(LogType logType, string message)
        {
            EventLog log = EventLog.Create(logType, message);
            this.loggerImpl.PushLog(log);            
        }

        public void LogInfo(string message)
        {
            this.Log(LogType.Info, message);
        }

        public void LogWarning(string message)
        {
            this.Log(LogType.Warning, message);
        }

        public void LogError(string message)
        {
            this.Log(LogType.Error, message);
        }        
    }
}
