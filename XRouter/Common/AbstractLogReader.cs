using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DaemonNT.Logging;

namespace XRouter.Common
{
    /// <summary>
    /// Provides read access to previously logged log entries.
    /// </summary>
    /// <remarks>
    /// This is an abstract base class which provides some common utility
    /// functions. The concrete implementation of parsing the log files must
    /// be defined in a derived class in the GetEntriesFromFile() method.
    /// </remarks>
    public abstract class AbstractLogReader<EntryType>
    {
        /// <summary>
        /// Wild-card pattern of log files to be scanned (within a single
        /// directory).
        /// </summary>
        protected string LogFilePattern = "*.log";

        /// <summary>
        /// Directory containing the log files to be scanned.
        /// </summary>
        protected string LogFilesDirectory { get; set; }

        /// <summary>
        /// Creates a new instance of the log reader.
        /// </summary>
        /// <param name="logFilesDirectory">directory containing the log files
        /// to be scanned; it can contain either absolute path or relative
        /// path related to the location of assembly executing the function
        /// </param>
        public AbstractLogReader(string logFilesDirectory)
        {
            LogFilesDirectory = logFilesDirectory;
            if (!Path.IsPathRooted(LogFilesDirectory)) {
                string binPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                LogFilesDirectory = Path.Combine(binPath, LogFilesDirectory);
            }
        }

        /// <summary>
        /// Scans the log files and obtains log entries matching the specified
        /// criteria. The log entries are paged.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If you want to select all entries (non-paged) set pageSize to
        /// int.MaxValue and pageNumber to 1.
        /// </para>
        /// <para>
        /// The log entries are assumed to be already ordered by date and time
        /// ascendantly.
        /// </para>
        /// </remarks>
        /// <param name="minDate">earliest date and time (inclusive) of log
        /// entries to be selected</param>
        /// <param name="maxDate">latest date and time (exclusive) of log
        /// entries to be selected</param>
        /// <param name="logLevelFilter">log level flags of selected log
        /// entries</param>
        /// <param name="pageSize">size of one page of selected log entries
        /// </param>
        /// <param name="pageNumber">1-based index of the page of selected log
        /// entries</param>
        /// <returns>the selected page of log entries</returns>
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
                    try
                    {
                        GetEntriesFromFile(logFilePath, date, minDate, maxDate,
                            logLevelFilter, ref matchingEntries);
                    }
                    catch (Exception ex)
                    {
                        TraceLog.Warning(string.Format(
                            "Cannot read log file: {0}. Reason: {1}",
                            Path.GetFileName(logFilePath), ex.Message));
                    }
                }
            }

            int entriesToSkip = (pageNumber - 1) * pageSize;
            var result = matchingEntries.Skip(entriesToSkip).Take(pageSize).ToArray();
            return result;
        }

        /// <summary>
        /// Scans a single file for log entries filtered by provided criteria
        /// and puts the matching entries to the provided container.
        /// </summary>
        /// <param name="logFilePath">path to the log file to be scanned</param>
        /// <param name="date">date of the log file (useful when the log
        /// entries themselves are not dates)</param>
        /// <param name="minDate">earliest date and time of log entries to be
        /// selected</param>
        /// <param name="maxDate">lasted date and time of log entries to be
        /// selected</param>
        /// <param name="logLevelFilter">log level flags of selected log
        /// entries</param>
        /// <param name="matchingEntries">prepared container where to put the
        /// selected log entries</param>
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
