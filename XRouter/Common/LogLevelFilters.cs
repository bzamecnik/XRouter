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
        [EnumMember]
        None = 0,
        [EnumMember]
        Info = 1,
        [EnumMember]
        Warning = 2,
        [EnumMember]
        Error = 4
    }
}
