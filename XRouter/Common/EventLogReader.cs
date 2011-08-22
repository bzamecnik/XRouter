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
            string[] lines = File.ReadAllLines(logFilePath);
            foreach (string line in lines)
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
            }
        }
    }
}
