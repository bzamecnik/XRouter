namespace DaemonNT.Logging
{
    using System;
    using System.IO;
    using System.Text;

    internal class LoggerFileStorage
    {
        private string source = null;

        private string directory = null;

        private StreamWriter streamWriter = null;

        private DateTime lastLoggedDateTime = DateTime.MinValue;

        private string lastLoggedFileName = null;
        
        public LoggerFileStorage(string source)
        {
            this.source = source;
            this.directory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
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

                // vytvori adresar, pokud neexistuje
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
