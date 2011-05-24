using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common
{
    public class TraceLog
    {
        private static DaemonNT.Logging.Logger daemonNTLogger;

        internal static void Initialize(DaemonNT.Logging.Logger daemonNTLogger)
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
