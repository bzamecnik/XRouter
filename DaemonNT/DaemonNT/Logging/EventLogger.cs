namespace DaemonNT.Logging
{
    using System;

    public class EventLogger
    {
        private LoggerFileStorage storage = null;

        private object lockObj = null;

        private EventLogger()
        {
        }

        public void Log(LogType logType, string message)
        {
            DateTime dateTime = DateTime.Now;
            string dateTimeStr = DateTime.Now.ToString("HH:mm:ss.ff");

            string logTypeStr = "I";
            switch (logType)
            {
                case LogType.Warning:
                    logTypeStr = "W";
                    break;
                case LogType.Error:
                    logTypeStr = "E";
                    break;
                // TODO: default
            }

            string log = string.Format("{0}\t{1}\t{2}", dateTimeStr, logTypeStr, message);

            lock (this.lockObj)
            {
                this.storage.Save(dateTime, log);
            }
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

        internal static EventLogger Start(string serviceName)
        {
            EventLogger instance = new EventLogger();
            instance.lockObj = new object();
            instance.storage = new LoggerFileStorage(serviceName);

            return instance;
        }

        internal void Stop()
        {
        }
    }
}
