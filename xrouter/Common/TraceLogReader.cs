using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DaemonNT.Logging;

namespace XRouter.Common
{
    /// <summary>
    /// Provides read access to previously logged trace log entries.
    /// </summary>
    public class TraceLogReader
    {
        private static readonly string LogFilePattern = "*_xrouter.Trace.log";

        private string LogFilesDirectory { get; set; }

        public TraceLogReader(string logFilesDirectory)
        {
            LogFilesDirectory = logFilesDirectory;
            if (!Path.IsPathRooted(LogFilesDirectory)) {
                string binPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                LogFilesDirectory = Path.Combine(binPath, LogFilesDirectory);
            }
        }

        public TraceLogEntry[] GetEntries(DateTime minDate, DateTime maxDate, LogLevelFilters logLevelFilter, int pageSize, int pageNumber)
        {
            List<TraceLogEntry> matchingEntries = new List<TraceLogEntry>();
            
            foreach (string logFilePath in Directory.GetFiles(LogFilesDirectory, LogFilePattern)) {
                string fileName = Path.GetFileName(logFilePath);
                DateTime date = GetDateFromLogFileName(fileName);
                if ((date >= minDate) && (date <= maxDate)) {
                    string fileContent = File.ReadAllText(logFilePath);
                    string xmlContent = "<log>" + fileContent + "</log>";
                    XDocument xLog = XDocument.Parse(xmlContent);
                    foreach (XElement xEntry in xLog.Root.Elements()) {
                        TraceLogEntry entry = new TraceLogEntry(xEntry);
                        if ((entry.Created >= minDate) && (entry.Created <= maxDate) && EventLogReader.IsMatchingLogLevelFilter(entry.LogLevel, logLevelFilter)) {
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
                    throw new ArgumentException(string.Format(
                        "Unknown log level '{0}'.", logLevel), "logLevel");
            }
        }
    }
}
