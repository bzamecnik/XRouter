using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common
{
    public class EventLog
    {
        private static DaemonNT.Logging.Logger daemonNTLogger;

        internal static void Initialize(DaemonNT.Logging.Logger daemonNTLogger)
        {
            EventLog.daemonNTLogger = daemonNTLogger;
        }

        public static void Info(string message)
        {
            daemonNTLogger.Event.LogInfo(message);
        }

        public static void Warning(string message)
        {
            daemonNTLogger.Event.LogWarning(message);
        }

        public static void Error(string message)
        {
            daemonNTLogger.Event.LogError(message);
        }
    }
}
