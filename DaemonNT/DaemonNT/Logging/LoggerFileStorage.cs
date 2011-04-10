namespace DaemonNT.Logging
{
    using System;
    using System.IO;
    using System.Text;

    internal class LoggerFileStorage
    {
        /// <summary>
        /// A symbolic name of the service using the logger.
        /// </summary>
        private string source;

        /// <summary>
        /// Absolute path to a directory where to store the log files.
        /// </summary>
        private string directory;

        private StreamWriter streamWriter;

        private DateTime lastLoggedDateTime = DateTime.MinValue;

        private string lastLoggedFileName;

        /// <summary>
        /// Relative directory name where the log files will be stored.
        /// </summary>
        /// <remarks>
        /// Currently it is based on the base directory of asseblies.
        /// TODO: this should not be hard-coded, but rather configured,
        /// eg. in the config file.
        /// </remarks>
        private static readonly string LOG_RELATIVE_DIRECTORY = "Logs";

        public LoggerFileStorage(string source)
        {
            this.source = source;
            this.directory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LOG_RELATIVE_DIRECTORY);
        }

        public void Save(DateTime logDateTime, string logItem)
        {
            string fileName = this.GetFileName(logDateTime);

            if (this.lastLoggedFileName != fileName)
            {
                if (this.streamWriter != null)
                {
                    this.streamWriter.Close();
                }

                // create directory if it does not exist
                if (!Directory.Exists(this.directory))
                {
                    Directory.CreateDirectory(this.directory);
                }

                this.streamWriter = new StreamWriter(fileName, true, Encoding.UTF8);
            }

            this.streamWriter.WriteLine(logItem);
            this.streamWriter.Flush();

            this.lastLoggedFileName = fileName;
            this.lastLoggedDateTime = logDateTime;
        }

        /// <summary>
        /// Vraci aktualni nazev log souboru. 
        /// </summary>
        /// <param name="logDateTime">DateTime kdy bylo skutecne zalogovano.</param>
        /// <returns></returns>
        private string GetFileName(DateTime logDateTime)
        {
            string fileName = this.lastLoggedFileName;

            if (logDateTime.Date != this.lastLoggedDateTime.Date)
            {
                fileName = Path.Combine(this.directory, string.Format("{0}_{1}.log", logDateTime.ToString("yyyy-MM-dd"), this.source));
            }

            return fileName;
        }
    }
}
