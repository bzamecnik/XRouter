using System;
using System.Runtime.Serialization;

namespace XRouter.Common
{
    [DataContract]
    [Flags]
    public enum LogLevelFilters
    {
        Info,
        Warning,
        Error
    }
}
