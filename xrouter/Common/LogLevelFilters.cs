using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
