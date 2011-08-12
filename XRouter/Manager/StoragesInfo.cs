using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Manager
{
    /// <summary>
    /// Objekt obsahuje informace potrebne pro pristup k persistentnim zdrojum. 
    /// </summary>
    internal sealed class StoragesInfo
    {
        /// <summary>
        /// ConnectionString k databazi XRouter. 
        /// </summary>
        public string DbConnectionString { private set; get; }

        /// <summary>
        /// DaemonNT Logs directory. 
        /// </summary>
        public string LogsDirectory { set; get; }

        public StoragesInfo(string dbConnectionString, string logsDirectory)
        {
            this.DbConnectionString = dbConnectionString;
            this.LogsDirectory = logsDirectory;
            if (this.LogsDirectory == null)
            {
                this.LogsDirectory = "Logs";
            }
        }
    }
}
