using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DaemonNT.Logging;
using XRouter.Common;

namespace XRouter.Manager
{
    /// <summary>
    /// Reprezentuje jednoduchy server, ktery posila e-mailove reporty. Report je odeslan 
    /// jednou za den v nastaveny cas. V reportu jsou pocty errors a warnings v event a
    /// trace logovych zaznamech a to poslednich 24 hodin k casu generovani reportu. 
    /// </summary>
    internal sealed class Reporter
    {
        /// <summary>
        /// Nazev instance asociovane XRouterService.
        /// </summary>
        private string serviceName = null;

        /// <summary>
        /// Informace pro pristup k persistentnim zdrojum. 
        /// </summary>
        private StoragesInfo storagesInfo = null;

        /// <summary>
        /// Odkaz na odesilac emailu. 
        /// </summary>
        private EMailSender sender = null;

        /// <summary>
        /// Odkaz na Daemon trace logger.
        /// </summary>
        private TraceLogger logger = null;

        /// <summary>
        /// Urcuje, jestli stale reportovani bezi. 
        /// </summary>
        private volatile bool isWorkerRunning = true;

        /// <summary>
        /// Thread, ktery hostuje implementaci reportingu. 
        /// </summary>
        private Thread worker = null;

        /// <summary>
        /// Perioda sledovani (polling) case v ms. 
        /// Hodnota je urcena implementaci a nebude parametrizovana.
        /// </summary>
        private static readonly int Interval = 1000;

        /// <summary>
        /// DateTime posledniho generovani reportu. 
        /// </summary>
        private DateTime LastReporting = DateTime.MinValue;

        /// <summary>
        /// Nastroj pro skenovani event logu.
        /// </summary>
        private EventLogReader eventLogReader = null;

        /// <summary>
        /// Nastroj pro skenovani trace logu. 
        /// </summary>
        private TraceLogReader traceLogReader = null;

        /// <summary>
        /// Cas generovani reportu. 
        /// </summary>
        private TimeSpan time;

        public Reporter(string serviceName, StoragesInfo storagesInfo, EMailSender sender, TraceLogger logger, TimeSpan time)
        {
            this.serviceName = serviceName;
            this.storagesInfo = storagesInfo;
            this.sender = sender;
            this.logger = logger;
            this.time = time;
        }

        public void Start()
        {
            // init log readers
            this.eventLogReader = new EventLogReader(this.storagesInfo.LogsDirectory);
            this.traceLogReader = new TraceLogReader(this.storagesInfo.LogsDirectory);

            this.worker = new Thread(delegate(object data)
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
                                DateTime max = new DateTime(dt.Year, dt.Month, dt.Day, time.Hours, time.Minutes, 0);                                
                                DateTime min = max.AddDays(-1);                                
                                string report = this.CreateReport(min, max);

                                // send report
                                if (this.sender != null)
                                {
                                    this.sender.Send("Report", report);
                                }
                            }
                            catch (Exception e)
                            {
                                this.logger.LogException(e);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        this.logger.LogException(e);
                    }
                }
            });

            this.worker.Start();
        }

        private string CreateReport(DateTime min, DateTime max)
        {
            EventLogEntry[] errorEventLogs = this.eventLogReader.GetEntries(min, max, LogLevelFilters.Error, int.MaxValue, 1);
            EventLogEntry[] warningEventLogs = this.eventLogReader.GetEntries(min, max, LogLevelFilters.Warning, int.MaxValue, 1);
            TraceLogEntry[] errorTraceLogs = this.traceLogReader.GetEntries(min, max, LogLevelFilters.Error, int.MaxValue, 1);
            TraceLogEntry[] warningTraceLogs = this.traceLogReader.GetEntries(min, max, LogLevelFilters.Warning, int.MaxValue, 1);

            StringBuilder sb = new StringBuilder();
            sb.Append("Event Logs:");
            sb.AppendLine();
            sb.Append(string.Format("ERRORs: {0}", errorEventLogs.Length));
            sb.AppendLine();
            sb.Append(string.Format("WARNINGs: {0}", warningEventLogs.Length));
            sb.AppendLine();
            sb.Append("Trace Logs:");
            sb.AppendLine();
            sb.Append(string.Format("ERRORs: {0}", errorTraceLogs.Length));
            sb.AppendLine();
            sb.Append(string.Format("WARNINGs: {0}", warningTraceLogs.Length));
            sb.AppendLine();

            return sb.ToString();
        }

        public void Stop()
        {
            this.isWorkerRunning = false;
            this.worker.Join();
        }
    }
}
