using System;
using DaemonNT;
using DaemonNT.Logging;

namespace XRouter.Manager
{
    /// <summary>
    /// A DaemonNT service running the XRouter manager - a collection of
    /// services for monitoring and management of a single XRouter service
    /// instance.
    /// </summary>
    /// <remarks>
    /// The services contained in an XRouter manager are: ConsoleServer for
    /// management of an XRouter service, Watcher monitoring and automatic
    /// restarting an XRouter service and possibly restarting it, and Reporter
    /// which send some summary reports via e-mail to an administrator.
    /// </remarks>
    public class XRouterManagerService : Service
    {
        private Reporter reporter;

        private Watcher watcher;

        private ConsoleServer consoleServer;

        protected override void OnStart(OnStartServiceArgs args)
        {
            // managedService (required)   
            string serviceName = args.Settings.Parameters["managedServiceName"];
            if (serviceName == null)
            {
                throw new ArgumentNullException("managedServiceName");
            }

            // E-MailSender (optional) 
            EMailSender emailSender = null;
            if (args.Settings["email"] != null)
            {
                emailSender = CreateEmailSender(args, this.Logger.Trace);
            }
            else
            {
                this.Logger.Trace.LogWarning("E-mail notification sender has not been initialized.");
            }

            // Storages (required)
            StoragesInfo storagesInfo = CreateStoragesInfo(args);

            // Reporter (required)
            this.reporter = CreateReporter(serviceName, args, storagesInfo, emailSender, this.Logger.Trace);

            // ServiceWatcher (required)
            this.watcher = CreateServiceWatcher(serviceName, args, emailSender, this.Logger.Trace);

            // ConsoleServer (required)
            this.consoleServer = CreateConsoleServer(serviceName, args, this.watcher, storagesInfo, this.Logger.Trace);

            // start servers            
            this.watcher.Start();
            this.consoleServer.Start();
            this.reporter.Start();
        }

        protected override void OnStop(OnStopServiceArgs args)
        {
            this.consoleServer.Stop();
            this.watcher.Stop();
            this.reporter.Stop();
        }

        /// <summary>
        /// Create and initialize an e-mail sender.
        /// </summary>
        /// <param name="args">service arguments with settings</param>
        /// <param name="logger">trace logger</param>
        /// <returns>EMailSender instance</returns>
        private static EMailSender CreateEmailSender(OnStartServiceArgs args, TraceLogger logger)
        {
            // SMTP host
            string smtpHost = args.Settings["email"].Parameters["smtpHost"];
            if (smtpHost == null)
            {
                throw new ArgumentNullException("smtpHost");
            }

            // SMTP port (optional)
            string smtpPort = args.Settings["email"].Parameters["smtpPort"];
            int? port = null;
            if (smtpPort != null)
            {
                port = Convert.ToInt32(smtpPort);
            }

            // From
            string from = args.Settings["email"].Parameters["from"];
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }
            System.Net.Mail.MailAddress fromAddress = new System.Net.Mail.MailAddress(from);

            // To
            string to = args.Settings["email"].Parameters["to"];
            if (to == null)
            {
                throw new ArgumentNullException("to");
            }
            System.Net.Mail.MailAddress toAddress = new System.Net.Mail.MailAddress(to);

            return new EMailSender(smtpHost, port, fromAddress, toAddress, logger);
        }

        /// <summary>
        /// Creates and initializes a storage information provider.
        /// </summary>
        /// <param name="args">service arguments with settings</param>
        /// <returns>StoragesInfo instance</returns>
        private static StoragesInfo CreateStoragesInfo(OnStartServiceArgs args)
        {
            if (args.Settings["storages"] == null)
            {
                throw new ArgumentNullException("storages");
            }

            // Connection String
            string connectionString = args.Settings["storages"].Parameters["connectionString"];
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }

            // Logs
            string logs = args.Settings["storages"].Parameters["logs"];

            return new StoragesInfo(connectionString, logs);
        }

        /// <summary>
        /// Creates and initializes a reporter.
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="args"></param>
        /// <param name="storagesInfo"></param>
        /// <param name="sender"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static Reporter CreateReporter(string serviceName, OnStartServiceArgs args, StoragesInfo storagesInfo, EMailSender sender, TraceLogger logger)
        {
            if (args.Settings["reporter"] == null)
            {
                throw new ArgumentNullException("reporter");
            }

            // Time (required)
            string time = args.Settings["reporter"].Parameters["time"];
            if (time == null)
            {
                throw new ArgumentNullException("time");
            }
            TimeSpan timeSpan = TimeSpan.Parse(time);

            return new Reporter(serviceName, storagesInfo, sender, logger, timeSpan);
        }

        /// <summary>
        /// Creates and initializes a service watcher.
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="args"></param>
        /// <param name="sender"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static Watcher CreateServiceWatcher(string serviceName, OnStartServiceArgs args, EMailSender sender,
            TraceLogger logger)
        {
            if (args.Settings["watcher"] == null)
            {
                throw new ArgumentNullException("watcher");
            }

            // Auto-restart enabled (required)
            string autoRestartEnabledStr = args.Settings["watcher"].Parameters["autoRestartEnabled"];
            if (autoRestartEnabledStr == null)
            {
                throw new ArgumentNullException("autoRestartEnabled");
            }
            bool autoRestartEnabled = Convert.ToBoolean(autoRestartEnabledStr);

            return new Watcher(serviceName, args.IsDebugMode, autoRestartEnabled, logger, sender);
        }

        /// <summary>
        /// Creates and initializes a console server.
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="args"></param>
        /// <param name="watcher"></param>
        /// <param name="storagesInfo"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static ConsoleServer CreateConsoleServer(string serviceName, OnStartServiceArgs args, Watcher watcher,
            StoragesInfo storagesInfo, TraceLogger logger)
        {
            if (args.Settings["console"] == null)
            {
                throw new ArgumentNullException("console");
            }

            // Web service URI (required)
            string uri = args.Settings["console"].Parameters["uri"];
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            // Metadata web service URI (required)
            string metadataUri = args.Settings["console"].Parameters["metadataUri"];
            if (uri == null)
            {
                throw new ArgumentNullException("metadataUri");
            }

            return new ConsoleServer(serviceName, args.IsDebugMode, uri, metadataUri, storagesInfo, watcher, logger);
        }
    }
}
