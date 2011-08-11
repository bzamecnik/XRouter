using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaemonNT;
using DaemonNT.Logging;

namespace XRouterManager
{
    /// <summary>
    /// Reprezentace sluzby XRouterManager, ktera poskytuje podpurne servery pro monitoring a management 
    /// sluzby XRouterService.
    /// </summary>
    public class XRouterManagerService : Service
    {
        private Reporter reporter = null;

        private Watcher watcher = null;

        private ConsoleServer consoleServer = null;
                      
        protected override void OnStart(OnStartServiceArgs args)
        {
            // XRouterServiceName (required)   
            string serviceName = args.Settings.Parameters["xrouterservice"];
            if (serviceName == null)
            {
                throw new ArgumentNullException("xrouterservice");
            }

            // E-MailSender (optional) 
            EMailSender emailSender = null;
            if (args.Settings["email"] != null)
            {
                emailSender = CreateEmailSender(args, this.Logger.Trace);                               
            }
            else
            {
                this.Logger.Trace.LogWarning("E-mail notification sender is not initialized.");
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

        /// <summary>
        /// Podpurna metoda pro vytvoreni a inicializaci instance EMailNotificationSender. 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static EMailSender CreateEmailSender(OnStartServiceArgs args, TraceLogger logger)
        {                  
            // SmtpHost
            string smtpHost = args.Settings["email"].Parameters["smtphost"];
            if (smtpHost == null)
            {
                throw new ArgumentNullException("smtphost");
            }

            // SmtpPort (optional)
            string smtpPort = args.Settings["email"].Parameters["smtpport"];
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
        /// Podpurna metoda pro vytvoreni a inicializaci instance StoragesInfo. 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static StoragesInfo CreateStoragesInfo(OnStartServiceArgs args)
        {
            if (args.Settings["storages"] == null)
            {
                throw new ArgumentNullException("storages");
            }

            // Connection String
            string connectionString = args.Settings["storages"].Parameters["connectionstring"];
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionstring");
            }

            // Logs
            string logs = args.Settings["storages"].Parameters["logs"];
            
            return new StoragesInfo(connectionString, logs);
        }

        /// <summary>
        /// Podpurna metoda pro vytvoreni a inicializaci instance Reporteru. 
        /// </summary>
        /// <param name="serviceName"></param>
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
        /// Podpurna metoda pro vytvoreni a inicializaci instance ServiceWatcher. 
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

            // ThrowUp (required)
            string throwUp = args.Settings["watcher"].Parameters["throwup"];
            if (throwUp == null)
            {
                throw new ArgumentNullException("throwup");
            }
            bool throwUpService = Convert.ToBoolean(throwUp);

            return new Watcher(serviceName, args.IsDebugMode, throwUpService, logger, sender); 
        }

        /// <summary>
        /// Podpurna metoda pro vytvoreni a inicializaci instance ConsoleServer. 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="args"></param>
        /// <param name="watcher"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private static ConsoleServer CreateConsoleServer(string serviceName, OnStartServiceArgs args, Watcher watcher, 
            StoragesInfo storagesInfo, TraceLogger logger)
        {
            if (args.Settings["console"] == null)
            {
                throw new ArgumentNullException("console");
            }

            // Uri (required)
            string uri = args.Settings["console"].Parameters["uri"];
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            // Metadata-Uri (required)
            string metadataUri = args.Settings["console"].Parameters["metadatauri"];
            if (uri == null)
            {
                throw new ArgumentNullException("metadatauri");
            }

            return new ConsoleServer(serviceName, args.IsDebugMode, uri, metadataUri, storagesInfo,  watcher, logger);
        }
                
        protected override void OnStop(OnStopServiceArgs args)
        {            
            this.consoleServer.Stop();
            this.watcher.Stop();
            this.reporter.Stop();
        }
    }
}
