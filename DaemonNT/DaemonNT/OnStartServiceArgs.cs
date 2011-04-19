using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaemonNT
{
    public sealed class OnStartServiceArgs
    {
        /// <summary>
        /// Identifikator sluzby v konfiguracnim souboru, resp. operacnim systemu.  
        /// </summary>
        public string ServiceName { internal set; get; }

        /// <summary>
        /// Urcuje, jestli je instance hostovana DaemonNT ladicim prostredim. 
        /// </summary>
        public bool IsDebugMode { internal set; get; }
       
        /// <summary>
        /// Poskytuje nastaveni sluzby definovane v konfiguracnim souboru. 
        /// </summary>
        public DaemonNT.Configuration.Settings Settings { internal set; get; }

        internal OnStartServiceArgs()
        { }
    }
}
