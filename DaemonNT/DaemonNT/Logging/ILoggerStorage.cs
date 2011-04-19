using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaemonNT.Logging
{
    internal interface ILoggerStorage
    {
        void SaveLog(Log log);
    }
}
