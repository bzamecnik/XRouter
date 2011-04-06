using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaemonNT.Logging
{
    public class EventLogger
    {
        private LoggerFileStorage storage = null;

        private Object lockObj = null;

        private EventLogger()
        { }

        internal static EventLogger Start(String serviceName)
        {
            EventLogger instance = new EventLogger();
            instance.lockObj = new Object();
            instance.storage = new LoggerFileStorage(serviceName);

            return instance;
        }

        internal void Stop()
        {

        }
        
        public void Log(LogType logType, String message)
        {
            DateTime dateTime = DateTime.Now;
            String sDateTime = DateTime.Now.ToString("HH:mm:ss.ff");

            String slogType = "I";
            switch (logType)
            {
                case LogType.Warning:
                    slogType = "W";
                    break;
                case LogType.Error:
                    slogType = "E";
                    break;
            }

            String log = String.Format("{0}\t{1}\t{2}", sDateTime, slogType, message);

            lock (lockObj)
            {
                this.storage.Save(dateTime, log);
            }
        }
        
        public void LogInfo(String message)
        {
            this.Log(LogType.Info, message);
        }

        public void LogWarning(String message)
        {
            this.Log(LogType.Warning, message);
        }

        public void LogError(String message)
        {
            this.Log(LogType.Error, message);
        }     
    }
}
