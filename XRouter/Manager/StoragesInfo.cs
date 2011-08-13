namespace XRouter.Manager
{
    /// <summary>
    /// Represents information needed for accessing various persistent
    /// resources.
    /// </summary>
    internal sealed class StoragesInfo
    {
        /// <summary>
        /// XRouter database connection string. The format is dependent on
        /// the concrete database used.
        /// </summary>
        public string DbConnectionString { private set; get; }

        /// <summary>
        /// Directory where DaemonNT should store log files.
        /// </summary>
        public string LogsDirectory { set; get; }

        public static readonly string DefaultLogsDirectory = "Logs";

        public StoragesInfo(string dbConnectionString, string logsDirectory)
        {
            this.DbConnectionString = dbConnectionString;
            this.LogsDirectory = logsDirectory;
            if (this.LogsDirectory == null)
            {
                this.LogsDirectory = DefaultLogsDirectory;
            }
        }
    }
}
