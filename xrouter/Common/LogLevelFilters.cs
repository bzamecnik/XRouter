using System;
using System.Runtime.Serialization;

namespace XRouter.Common
{
    /// <summary>
    /// Flags which mark one or more allowed log levels from
    /// <see cref="DaemonNT.Logging.LogType"/>.
    /// </summary>
    [DataContract]
    [Flags]
    public enum LogLevelFilters
    {
        Info,
        Warning,
        Error
    }
}
