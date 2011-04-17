namespace DaemonNT.Logging
{
    using System.Text;
    using System.Threading;
 
    public sealed class TraceLogger
    {
        private LoggerImplementation loggerImpl = null;

        private TraceLogger()
        { }

        internal static TraceLogger Create(string serviceName, int bufferSize)
        {
            TraceLogger instance = new TraceLogger();            
            LoggerFileStorage storage = new LoggerFileStorage(string.Format("{0}.Trace", serviceName));
            instance.loggerImpl = LoggerImplementation.Start(storage, bufferSize);

            return instance;
        }
       
        internal void Close()
        {
            this.loggerImpl.Stop();
        }
 
        public void Log(string content)
        {
            TraceLog log = TraceLog.Create(content);
            this.loggerImpl.PushLog(log);            
        }         
    }
}
