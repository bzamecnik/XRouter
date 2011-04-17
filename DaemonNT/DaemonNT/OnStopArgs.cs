using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaemonNT
{   
    public sealed class OnStopArgs
    {
        /// <summary>
        /// Urcuje, jestli je sluzba zastavena vypnutim instance operacniho systemu. 
        /// </summary>
        public Boolean Shutdown { internal set; get; }

        internal OnStopArgs()
        { }
    }
}
