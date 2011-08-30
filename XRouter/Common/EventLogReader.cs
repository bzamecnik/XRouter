using System;
using System.Collections.Generic;
using System.IO;

namespace XRouter.Common
{
    /// <summary>
    /// Provides read access to previously logged event log entries.
    /// </summary>
    public class EventLogReader : AbstractLogReader<EventLogEntry>
    {
        public EventLogReader(string logFilesDirectory, string serviceName)
            : base(logFilesDirectory)
        {
            LogFilePattern = string.Format("*_{0}.log", serviceName);
        }

        protected override void GetEntriesFromFile(
            string logFilePath,
            DateTime date,
            DateTime minDate,
            DateTime maxDate,
            LogLevelFilters logLevelFilter,
            ref List<EventLogEntry> matchingEntries)
        {
            using (FileStream fileStream = new FileStream(logFilePath,
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line.Trim().Length == 0)
                    {
                        // ignore empty lines
                        continue;
                    }
                    EventLogEntry entry = new EventLogEntry(line, date);
                    if ((entry.Created >= minDate) && (entry.Created <= maxDate) &&
                        IsMatchingLogLevelFilter(entry.LogLevel, logLevelFilter))
                    {
                        matchingEntries.Add(entry);
                    }
                    line = reader.ReadLine();
                }
            }
        }
    }
}
