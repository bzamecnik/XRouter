using System;
using DaemonNT.Logging;

namespace XRouter.Common
{
    /// <summary>
    /// Provides a log of structured data useful for tracing. Implemented
    /// using DaemonNT logging.
    /// </summary>
    /// <remarks>
    /// Must be initialized using the Initialize() method before usage.
    /// </remarks>
    public class TraceLog
    {
        private static Logger daemonNTLogger;

        public static void Initialize(Logger daemonNTLogger)
        {
            TraceLog.daemonNTLogger = daemonNTLogger;
        }

        public static void Info(string xmlContent)
        {
            daemonNTLogger.Trace.LogInfo(xmlContent);
        }

        public static void Warning(string xmlContent)
        {
            daemonNTLogger.Trace.LogWarning(xmlContent);
        }

        public static void Error(string xmlContent)
        {
            daemonNTLogger.Trace.LogError(xmlContent);
        }

        public static void Exception(Exception exception)
        {
            daemonNTLogger.Trace.LogException(exception);
        }
    }
}
