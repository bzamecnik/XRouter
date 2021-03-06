﻿using System;
using System.ServiceProcess;
using System.Threading;
using XRouter.Common;

namespace XRouter.Manager
{
    /// <summary>
    /// Periodically watches the status of a single managed Windows service
    /// and can automatically try to restart the service if seems to be down
    /// for some time.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The automatical restarts can be enabled in the contructor via the
    /// autoStartEnabled parameter (usually taken from the manager service
    /// configuration). Automatic restarting can be temporarily disabled using
    /// the DisableServiceAutoRestart() method. As soon as the service is
    /// started automatic restarting behaves again as configured.
    /// </para>
    /// <para>
    /// On each automatic restart an optional e-mail notification is sent.
    /// </para>
    /// <para>
    /// It is possible to get the last known state of the managed service via
    /// the ServiceStatus property.
    /// </para>
    /// <para>
    /// Currently, Watcher does not work when it is ran in DaemonNT debug mode.
    /// </para>
    /// </remarks>
    internal sealed class Watcher
    {
        /// <summary>
        /// DaemonNT service name of the managed XRouterService.
        /// </summary>
        private string managedServiceName = null;

        /// <summary>
        /// Indicates whether this XRouterManager service is ran in the DaemonNT
        /// debug mode.
        /// </summary>
        /// <remarks>
        /// When in debug mode the watcher does not work.
        /// </remarks>
        private bool isDebugMode = false;

        /// <summary>
        /// Indicated whether to auto-restart the managed service if it seems
        /// to be stopped for some time (as specified in configuration).
        /// </summary>
        private bool configAutoRestartEnabled = false;

        /// <summary>
        /// Indicated whether to auto-restart the managed service if it seems
        /// to be stopped for some time (can change at run-time).
        /// </summary>
        private volatile bool runtimeAutoRestartEnabled = false;

        /// <summary>
        /// Worker thread in which the watcher server runs.
        /// </summary>
        private Thread worker = null;

        /// <summary>
        /// Indicates whether the watcher is still running.
        /// </summary>
        /// <remarks>
        /// False means that the worker thread should finnish.
        /// </remarks>
        private volatile bool isWorkerRunning = true;

        /// <summary>
        /// The number of subseqent queries for status of the managed service
        /// with negative result (stopped). If it exceeds a threshold the
        /// managed service can be auto-restarted.
        /// </summary>
        private int stoppedTimes = 0;

        /// <summary>
        /// The last known status of the managed service.
        /// </summary>
        private volatile ServiceControllerStatus lastServiceStatus = ServiceControllerStatus.Stopped;

        /// <summary>
        /// Period of polling (in milliseconds) for the status of the
        /// managed service.
        /// </summary>
        /// <remarks>
        /// The value will not be configurable.
        /// </remarks>
        private static readonly int Interval = 1000;

        /// <summary>
        /// The maximum number of subseqent queries for status of the managed
        /// service with negative result (stopped) after which the managed
        /// service can be auto-restarted.
        /// </summary>
        /// <remarks>
        /// The value will not be configurable.
        /// </remarks>
        private static readonly int MaxStoppedTimes = 10;

        /// <summary>
        /// Reference to an e-mail sender.
        /// </summary>
        private EMailSender emailSender = null;

        /// <summary>
        /// The last known status of the managed service.
        /// </summary>
        public ServiceControllerStatus ServiceStatus
        {
            get { return this.lastServiceStatus; }
        }

        /// <summary>
        /// Disables restarting an unresponsible service automatically.
        /// </summary>
        public void DisableServiceAutoStart()
        {
            this.runtimeAutoRestartEnabled = false;
        }

        public Watcher(string serviceName, bool isDebugMode, bool autoStartEnabled, EMailSender sender)
        {
            this.managedServiceName = serviceName;
            this.isDebugMode = isDebugMode;
            this.configAutoRestartEnabled = autoStartEnabled;
            this.runtimeAutoRestartEnabled = autoStartEnabled;
            this.emailSender = sender;

            if (isDebugMode)
            {
                TraceLog.Warning("Watcher is disabled in debug mode.");
            }
        }

        /// <summary>
        /// Starts the watcher server in a new thread.
        /// </summary>
        public void Start()
        {
            this.worker = new Thread(RunWatcher);
            this.worker.Start(null);
        }

        /// <summary>
        /// Stops the watcher thread.
        /// </summary>
        public void Stop()
        {
            this.isWorkerRunning = false;
            this.worker.Join();
        }

        private void RunWatcher(object data)
        {
            while (this.isWorkerRunning)
            {
                Thread.Sleep(Interval);

                if (this.isDebugMode)
                {
                    continue;
                }
                try
                {
                    // get status
                    ServiceController sc = new ServiceController(this.managedServiceName);
                    this.lastServiceStatus = sc.Status;

                    // manage stopped times
                    if (sc.Status == ServiceControllerStatus.Stopped)
                    {
                        this.stoppedTimes++;
                    }
                    else
                    {
                        if (sc.Status == ServiceControllerStatus.Running)
                        {
                            this.runtimeAutoRestartEnabled = true;
                        }
                        this.stoppedTimes = 0;
                    }

                    // automatically restart the managed service
                    if (this.configAutoRestartEnabled && this.runtimeAutoRestartEnabled)
                    {
                        RestartAutomatically(sc);
                    }
                }
                catch (Exception e)
                {
                    TraceLog.Exception(e);
                }
            }
        }

        private void RestartAutomatically(ServiceController sc)
        {
            if (this.stoppedTimes == MaxStoppedTimes)
            {
                this.stoppedTimes = 0;

                // log info
                string logMessage = (string.Format(
                    "Watcher automatically restarted managed service '{0}'.",
                    this.managedServiceName));
                TraceLog.Info(logMessage);

                // send e-mail
                if (this.emailSender != null)
                {
                    this.emailSender.Send("Automatic restart", logMessage);
                }

                // start service
                sc.Start();
            }
        }
    }
}
