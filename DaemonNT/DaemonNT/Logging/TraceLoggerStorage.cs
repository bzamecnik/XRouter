
namespace DaemonNT.Logging
{
    /// <summary>
    /// Provides an abstract class for a DaemonNT trace logger storage that
    /// will exist as part of a service logger.
    /// </summary>
    public abstract class TraceLoggerStorage
    {
        /// <summary>
        /// Provides a facility for efficient, thread-safe logging of
        /// important events designated for service administrators.
        /// </summary>
        public EventLogger Event { internal set; get; }

        internal void Start(OnStartTraceLoggerStorageArgs args)
        {
            this.OnStart(args);
        }

        protected virtual void OnStart(OnStartTraceLoggerStorageArgs args)
        { }

        internal void SaveLog(TraceLog log)
        {
            this.OnSaveLog(log);
        }

        protected abstract void OnSaveLog(TraceLog log);

        internal void Stop(OnStopTraceLoggerStorageArgs args)
        {
            this.OnStop(args);
        }

        protected virtual void OnStop(OnStopTraceLoggerStorageArgs args)
        { }
    }
}
