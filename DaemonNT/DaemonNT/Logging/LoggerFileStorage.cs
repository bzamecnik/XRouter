using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DaemonNT.Logging
{
    internal class LoggerFileStorage
    {
        private String source = null;

        private String directory = null;

        private StreamWriter streamWriter = null;

        private DateTime lastLoggedDateTime = DateTime.MinValue;

        private String lastLoggedFileName = null;
        
        public LoggerFileStorage(String source)
        {
            this.source = source;
            this.directory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        }
      
        public void Save(DateTime logDateTime, String logItem)
        {
            String fileName = this.GetFileName(logDateTime);

            if (lastLoggedFileName != fileName)
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
        private String GetFileName(DateTime logDateTime)
        {
            String fileName = this.lastLoggedFileName;
 
            if (logDateTime.Date != this.lastLoggedDateTime.Date)
            {
                fileName = Path.Combine(this.directory, String.Format("{0}_{1}.log", logDateTime.ToString("yyyy-MM-dd"), this.source));                                            
            }

            return fileName;
        }        
    }
}
