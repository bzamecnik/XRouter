namespace DaemonNT.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using DaemonNT.Configuration;

    /// <summary>
    /// Provides a facility for efficient, thread-safe logging of
    /// potentially low-level information designated for developers or
    /// specialists in the problem domain.
    /// </summary>
    public sealed class TraceLogger
    {
        private LoggerImplementation loggerImpl = null;

        private ILoggerStorage[] storages = null;

        private TraceLogger()
        { }

        /// <summary>
        /// Creates and initializes an instance of the trace logger.
        /// </summary>
        /// <remarks>
        /// <para>
        /// After calling this method it is possible to start logging.
        /// </para>
        /// <para>
        /// Several storages can be used. In addition to a standard storage
        /// LoggerFileStorage other storages can be configured in settings.
        /// </para>
        /// </remarks>
        /// <param name="serviceName">Service name</param>
        /// <param name="bufferSize">Size of the buffer of log records (number
        /// of records).</param>
        /// <param name="isDebugMode">Indicates whether the services is ran in
        /// debug mode or as an NT service.</param>
        /// <param name="settings">Trace logger settings</param>
        /// <param name="eventLogger">Event logger for usage within storages.
        /// </param>
        /// <returns>Initialized trace logger instance.</returns>
        internal static TraceLogger Create(
            string serviceName,
            int bufferSize,
            bool isDebugMode,
            TraceLoggerSettings settings,
            EventLogger eventLogger)
        {
            TraceLogger instance = new TraceLogger();

            List<ILoggerStorage> storages = new List<ILoggerStorage>();

            // add standard storage
            storages.Add(new LoggerFileStorage(string.Format("{0}.Trace", serviceName)));

            // add configured storages
            foreach (TraceLoggerStorageSettings traceLoggerSettings in settings.Storages)
            {
                // create and initialize storage
                TraceLoggerStorage storage = TypesProvider.CreateTraceLoggerStorage(
                    traceLoggerSettings.TypeClass, traceLoggerSettings.TypeAssembly);
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
        /// Stops the trace logger.
        /// </summary>
        /// <remarks>
        /// It stops receiving logs, stops the storages, clears the buffers
        /// etc.</remarks>
        /// <param name="shutdown">Indicates whether the logger was stopped
        /// manually or during an operating system shutdown.</param>
        internal void Close(bool shutdown)
        {
            // stops receiving logs and waits for the buffer to be cleared
            this.loggerImpl.Stop();

            // notifies the configured trace logger storages to stop
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
        /// Creates a trace log record of given type and saves it into an
        /// internal buffer. The records are later moved to a persistent
        /// storage.
        /// </summary>
        /// <param name="logType">Type of the trace log record.</param>
        /// <param name="xmlContent">Any XML content which can be customized
        /// for the target problem domain in order to store stucured
        /// information.</param>
        public void Log(LogType logType, string xmlContent)
        {
            TraceLog log = TraceLog.Create(logType, xmlContent);
            this.loggerImpl.PushLog(log);
        }

        /// <summary>
        /// Creates an information type trace log record and saves it into an
        /// internal buffer. The records are later moved to a persistent
        /// storage.
        /// </summary>
        /// <remarks>
        /// The method is thread-safe.
        /// </remarks>
        /// <param name="xmlContent">Any XML content which can be customized
        /// for the target problem domain in order to store stucured
        /// information.</param>
        public void LogInfo(string xmlContent)
        {
            this.Log(LogType.Info, xmlContent);
        }

        /// <summary>
        /// Creates a warning type trace log record and saves it into an
        /// internal buffer. The records are later moved to a persistent
        /// storage.
        /// </summary>
        /// <param name="xmlContent">Any XML content which can be customized
        /// for the target problem domain in order to store stucured
        /// information.</param>
        public void LogWarning(string xmlContent)
        {
            this.Log(LogType.Warning, xmlContent);
        }

        /// <summary>
        /// Creates an error type trace log record and saves it into an
        /// internal buffer. The records are later moved to a persistent
        /// storage.
        /// </summary>
        /// <param name="xmlContent">Any XML content which can be customized
        /// for the target problem domain in order to store stucured
        /// information.</param>
        public void LogError(string xmlContent)
        {
            this.Log(LogType.Error, xmlContent);
        }

        /// <summary>
        /// Creates an error type trace log record containing a serialized
        /// exception and saves it into an internal buffer. The records are
        /// later moved to a persistent storage.
        /// </summary>
        /// <param name="exception">Exception to be serialized and logged.
        /// </param>
        public void LogException(Exception exception)
        {
            this.Log(LogType.Error, SerializeException(exception));
        }

        /// <summary>
        /// Serializes the exception and all its inner exceptions into a single
        /// XML content.
        /// </summary>
        /// <param name="exception">Exception to be serialized.</param>
        /// <returns>Exception serialized into XML.</returns>
        private String SerializeException(Exception exception)
        {
            StringBuilder sb = new StringBuilder();

            // type
            sb.Append(String.Format("<exception type=\"{0}\">", exception.GetType().ToString()));

            // message
            if (exception.Message != null)
            {
                sb.Append(String.Format("<message>{0}</message>", exception.Message));
            }

            // data
            if (exception.Data.Count > 0)
            {
                sb.Append("<data>");
                foreach (System.Collections.DictionaryEntry entry in exception.Data)
                {
                    String key = entry.Key.ToString();
                    String value = entry.Value.ToString();
                    sb.Append(String.Format("<entry key=\"{0}\" value=\"{1}\"/>", key, value));
                }
                sb.Append("</data>");
            }

            // stack-trace
            sb.Append(String.Format("<stack-trace>{0}</stack-trace>", exception.StackTrace));

            // inner exception
            if (exception.InnerException != null)
            {
                sb.Append(String.Format("{0}", SerializeException(exception.InnerException)));
            }

            sb.Append(String.Format("</exception>"));

            return sb.ToString();
        }
    }
}
