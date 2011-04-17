using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaemonNT.Logging
{
    public abstract class Log
    {
        public DateTime DateTime { get; protected set; }
    }
}
