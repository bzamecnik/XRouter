using System;
using System.Text;
using System.Threading;
using XRouter.Common;

namespace XRouter.Manager
{
    /// <summary>
    /// Reporter is a simple server which periodically generates summary
    /// reports about the manager XRouterService and sends them via e-mail.
    /// </summary>
    /// <remarks>
    /// A report e-mail is send once a day at a configured time. It contains
    /// the number of errors, warning in event log and trace log entries on
    /// the previous day.
    /// </remarks>
    internal sealed class Reporter
    {
        /// <summary>
        /// DaemonNT service name of the managed XRouterService.
        /// </summary>
        private string serviceName = null;

        /// <summary>
        /// Information for accessing persistent resources.
        /// </summary>
        private StoragesInfo storagesInfo = null;

        /// <summary>
        /// Reference to an e-mail sender.
        /// </summary>
        private EMailSender sender = null;

        /// <summary>
        /// Indicates whether the reporter is still running.
        /// </summary>
        /// <remarks>
        /// False means that the worker thread should finnish.
        /// </remarks>
        private volatile bool isWorkerRunning = true;

        /// <summary>
        /// Worker thread in which the reporter server runs.
        /// </summary>
        private Thread worker = null;

        /// <summary>
        /// Period (in milliseconds) for polling the changes in the logs.
        /// </summary>
        /// <remarks>
        /// The value will not be configurable.
        /// </remarks>
        private static readonly int Interval = 1000;

        /// <summary>
        /// Date and time when the last report was generated.
        /// </summary>
        private DateTime LastReporting = DateTime.MinValue;

        /// <summary>
        /// A tool for scanning the event log of the managed service.
        /// </summary>
        private EventLogReader eventLogReader = null;

        /// <summary>
        /// A tool for scanning the trace log of the managed service.
        /// </summary>
        private TraceLogReader traceLogReader = null;

        /// <summary>
        /// Time withing a day when a report should be generated.
        /// </summary>
        /// <remarks>
        /// Reports will be generated each day at this specified time.
        /// </remarks>
        private TimeSpan time;

        public Reporter(string serviceName, StoragesInfo storagesInfo, EMailSender sender, TimeSpan time)
        {
            this.serviceName = serviceName;
            this.storagesInfo = storagesInfo;
            this.sender = sender;
            this.time = time;
        }

        /// <summary>
        /// Starts the reporter server in a new thread.
        /// </summary>
        public void Start()
        {
            // init log readers
            this.eventLogReader = new EventLogReader(this.storagesInfo.LogsDirectory, this.serviceName);
            this.traceLogReader = new TraceLogReader(this.storagesInfo.LogsDirectory, this.serviceName);

            this.worker = new Thread(Run);
            this.worker.Start();
        }

        /// <summary>
        /// Stops the reporter server thread.
        /// </summary>
        public void Stop()
        {
            this.isWorkerRunning = false;
            this.worker.Join();
        }

        private void Run()
        {
            while (isWorkerRunning)
            {
                Thread.Sleep(Interval);
                try
                {
                    DateTime dt = DateTime.Now;
                    if (LastReporting.Date != dt.Date && dt.Hour == time.Hours && dt.Minute == time.Minutes)
                    {
                        LastReporting = dt;
                        try
                        {
                            DateTime max = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
                            DateTime min = max.AddDays(-1);
                            TraceLog.Info(string.Format("Creating report from logs at {0}", dt));
                            string report = this.CreateReport(dt, min, max);

                            // send report
                            if (this.sender != null)
                            {
                                this.sender.Send(string.Format(
                                    "Report for service '{0}' at {1}", serviceName, dt),
                                    report);
                                TraceLog.Info("Report was sent successfully.");
                            }
                        }
                        catch (Exception e)
                        {
                            TraceLog.Exception(e);
                        }
                    }
                }
                catch (Exception e)
                {
                    TraceLog.Exception(e);
                }
            }
        }

        private string CreateReport(DateTime now, DateTime min, DateTime max)
        {
            EventLogEntry[] errorEventLogs = this.eventLogReader.GetEntries(min, max, LogLevelFilters.Error, int.MaxValue, 1);
            EventLogEntry[] warningEventLogs = this.eventLogReader.GetEntries(min, max, LogLevelFilters.Warning, int.MaxValue, 1);
            TraceLogEntry[] errorTraceLogs = this.traceLogReader.GetEntries(min, max, LogLevelFilters.Error, int.MaxValue, 1);
            TraceLogEntry[] warningTraceLogs = this.traceLogReader.GetEntries(min, max, LogLevelFilters.Warning, int.MaxValue, 1);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format(
                "Summary of logs for service '{0}' between {1} and {2}:",
                serviceName, min, max));
            sb.AppendLine();
            sb.AppendLine("Event Logs:");
            sb.AppendLine(string.Format("  Errors: {0}", errorEventLogs.Length));
            sb.AppendLine(string.Format("  Warnings: {0}", warningEventLogs.Length));
            sb.AppendLine();
            sb.AppendLine("Trace Logs:");
            sb.AppendLine(string.Format("  Errors: {0}", errorTraceLogs.Length));
            sb.AppendLine(string.Format("  Warnings: {0}", warningTraceLogs.Length));
            sb.AppendLine();
            sb.AppendLine(string.Format("Report was generated at {0}.", now));

            return sb.ToString();
        }
    }
}
