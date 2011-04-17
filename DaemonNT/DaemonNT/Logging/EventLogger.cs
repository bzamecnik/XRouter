namespace DaemonNT.Logging
{
    using System;
    using System.Collections.Generic;
          
    public class EventLogger
    {
        private LoggerImplementation loggerImpl = null;
       
        private EventLogger()
        { }

        internal static EventLogger Create(String serviceName, Int32 bufferSize)
        {
            EventLogger instance = new EventLogger(); 
           
            LoggerFileStorage storage = new LoggerFileStorage(serviceName);
            instance.loggerImpl = LoggerImplementation.Start(storage, bufferSize);

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
