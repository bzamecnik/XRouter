namespace DaemonNT.Test
{
    using System;
    using System.Timers;
    using DaemonNT;
    using DaemonNT.Logging;

    /// <summary>
    /// An example trivial service implementation. It shows how to read
    /// settings and how to override the Service hook methods.
    /// </summary>
    /// <remarks>
    /// It writes a trace-log event in periodic intervals until it is stopped.
    /// </remarks>
    public class TestService : Service
    {
        private Timer timer = null;

        private Logger logger = null;

        protected override void OnStart(ServiceArgs args)
        {
            this.logger = args.Logger;

            int interval = Convert.ToInt32(args.Settings["timer"].Parameter["interval"]);

            this.timer = new Timer();
            this.timer.Interval = interval;
            this.timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            this.timer.AutoReset = false;
            this.timer.Enabled = true;

            this.logger.Trace.Log("Timer initialized!");
            this.logger.Event.LogInfo("Service started!");
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.logger.Trace.Log("Tick");
            // start the timer again
            this.timer.Enabled = true;
        }

        protected override void OnStop(bool shutdown)
        {
            this.logger.Event.LogInfo("Service stopped!");
        }
    }
}
