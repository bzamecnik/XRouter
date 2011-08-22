using DaemonNT.Logging;

namespace XRouter.Common
{
    /// <summary>
    /// Provides a log of important run-time events. Implemented using
    /// DaemonNT logging.
    /// </summary>
    /// <remarks>
    /// Must be initialized using the Initialize() method before usage.
    /// </remarks>
    public class EventLog
    {
        private static Logger daemonNTLogger;

        public static void Initialize(Logger daemonNTLogger)
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
