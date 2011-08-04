using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DaemonNT.Logging;

namespace XRouter.Common
{
    /// <summary>
    /// Provides read access to previously logged event log entries.
    /// </summary>
    public class EventLogReader
    {
        private static readonly string LogFilePattern = "*_xrouter.log";

        private string LogFilesDirectory { get; set; }

        public EventLogReader(string logFilesDirectory)
        {
            LogFilesDirectory = logFilesDirectory;
            if (!Path.IsPathRooted(LogFilesDirectory)) {
                string binPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                LogFilesDirectory = Path.Combine(binPath, LogFilesDirectory);
            }
        }

        public EventLogEntry[] GetEntries(DateTime minDate, DateTime maxDate, LogLevelFilters logLevelFilter, int pageSize, int pageNumber)
        {
            List<EventLogEntry> matchingEntries = new List<EventLogEntry>();
            
            foreach (string logFilePath in Directory.GetFiles(LogFilesDirectory, LogFilePattern)) {
                string fileName = Path.GetFileName(logFilePath);                
                DateTime date = GetDateFromLogFileName(fileName);
                if ((date >= minDate) && (date <= maxDate)) {
                    string[] lines = File.ReadAllLines(logFilePath);
                    foreach (string line in lines) {
                        EventLogEntry entry = new EventLogEntry(line, date);
                        if ((entry.Created >= minDate) && (entry.Created <= maxDate) && IsMatchingLogLevelFilter(entry.LogLevel, logLevelFilter)) {
                            matchingEntries.Add(entry);
                        }
                    }
                }
            }

            int entriesToSkip = (pageNumber - 1) * pageSize;
            var result = matchingEntries.Skip(entriesToSkip).Take(pageSize).ToArray();
            return result;
        }

        internal static DateTime GetDateFromLogFileName(string fileName)
        {
            int endPos = fileName.IndexOf('_');
            string dateText = fileName.Substring(0, endPos);
            string[] dateParts = dateText.Split('-');
            int year = int.Parse(dateParts[0]);
            int month = int.Parse(dateParts[1]);
            int day = int.Parse(dateParts[2]);
            return new DateTime(year, month, day);
        }

        internal static bool IsMatchingLogLevelFilter(LogType logLevel, LogLevelFilters filter)
        {
            switch (logLevel) {
                case LogType.Info:
                    return filter.HasFlag(LogLevelFilters.Info);
                case LogType.Warning:
                    return filter.HasFlag(LogLevelFilters.Warning);
                case LogType.Error:
                    return filter.HasFlag(LogLevelFilters.Error);
                default:
                    throw new ArgumentException("Unknown log level", "logLevel");
            }
        }
    }
}
