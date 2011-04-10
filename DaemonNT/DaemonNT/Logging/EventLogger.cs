namespace DaemonNT.Logging
{
    using System;

    public class EventLogger
    {
        private LoggerFileStorage storage;

        private object lockObj;

        /// <summary>
        /// Creates a new instance of the EventLogger class.
        /// </summary>
        /// <remarks>
        /// For creating a new instance use the Start() factory method.
        /// </remarks>
        /// <param name="storage"></param>
        private EventLogger(LoggerFileStorage storage)
        {
            this.lockObj = new object();
            this.storage = storage;
        }

        public void Log(LogType logType, string message)
        {
            DateTime dateTime = DateTime.Now;
            string dateTimeStr = dateTime.ToString("HH:mm:ss.ff");

            string logTypeStr;
            switch (logType)
            {
                case LogType.Info:
                    logTypeStr = "I";
                    break;
                case LogType.Warning:
                    logTypeStr = "W";
                    break;
                case LogType.Error:
                    logTypeStr = "E";
                    break;
                default:
                    throw new ArgumentException(String.Format(
                        "Unsupported log type: {0}", logType));
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

        /// <summary>
        /// Creates and starts a new logger instance.
        /// </summary>
        /// <remarks>
        /// This is a factory method.
        /// </remarks>
        /// <param name="serviceName">Name of the service which uses the
        /// logger.</param>
        /// <returns>A new instance of the logger.</returns>
        internal static EventLogger Start(string serviceName)
        {
            return new EventLogger(new LoggerFileStorage(serviceName));
        }

        internal void Stop()
        {
        }
    }
}
