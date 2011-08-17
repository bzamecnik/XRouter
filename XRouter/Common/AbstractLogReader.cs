using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DaemonNT.Logging;

namespace XRouter.Common
{
    public abstract class AbstractLogReader<EntryType>
    {
        protected string LogFilePattern = "*.log";

        protected string LogFilesDirectory { get; set; }

        public AbstractLogReader(string logFilesDirectory)
        {
            LogFilesDirectory = logFilesDirectory;
            if (!Path.IsPathRooted(LogFilesDirectory)) {
                string binPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                LogFilesDirectory = Path.Combine(binPath, LogFilesDirectory);
            }
        }

        public EntryType[] GetEntries(
            DateTime minDate,
            DateTime maxDate,
            LogLevelFilters logLevelFilter,
            int pageSize,
            int pageNumber)
        {
            List<EntryType> matchingEntries = new List<EntryType>();

            foreach (string logFilePath in Directory.GetFiles(LogFilesDirectory, LogFilePattern))
            {
                string fileName = Path.GetFileName(logFilePath);
                DateTime date = GetDateFromLogFileName(fileName);
                if ((date >= minDate) && (date <= maxDate))
                {
                    GetEntriesFromFile(logFilePath, date, minDate, maxDate,
                        logLevelFilter, ref matchingEntries);
                }
            }

            int entriesToSkip = (pageNumber - 1) * pageSize;
            var result = matchingEntries.Skip(entriesToSkip).Take(pageSize).ToArray();
            return result;
        }

        protected abstract void GetEntriesFromFile(
            string logFilePath,
            DateTime date,
            DateTime minDate,
            DateTime maxDate,
            LogLevelFilters logLevelFilter,
            ref List<EntryType> matchingEntries);

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
            switch (logLevel)
            {
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
