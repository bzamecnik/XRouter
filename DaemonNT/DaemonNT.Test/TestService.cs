namespace DaemonNT.Test
{
    using System;
    using System.Threading;
    using DaemonNT;
    using DaemonNT.Logging;

    /// <summary>
    /// An example trivial service implementation. It shows how to read
    /// settings and how to override the Service hook methods.
    /// </summary>
    /// <remarks>
    /// It writes trace-log events periodically until it is stopped.
    /// </remarks>
    public class TestService : Service
    {
        private Thread timer;

        private ManualResetEvent mreStopPending = null;

        protected override void OnStart(OnStartServiceArgs args)
        {
            // event log
            this.Logger.Event.LogInfo(String.Format("ServiceName={0} IsDebugMode={1}",
                args.ServiceName, args.IsDebugMode));

            // get settings
            int interval = 0;
            try
            {
                interval = Convert.ToInt32(args.Settings["timer"].Parameter["interval"]);
            }
            catch (Exception e)
            {
                this.Logger.Event.LogError(String.Format("Settings are invalid! {0}", e.Message));
                throw e;
            }

            // initialize and start timer
            this.timer = new Thread(new ParameterizedThreadStart(this.Tick));
            this.mreStopPending = new ManualResetEvent(false);
            this.timer.Start(interval);
        }

        private void Tick(Object data)
        {
            try
            {
                int interval = Convert.ToInt32(data);

                while (true)
                {
                    Thread.Sleep(interval);

                    // log some text
                    this.Logger.Trace.LogInfo("tick");

                    // log some complex content
                    DateTime dt = DateTime.Now;
                    this.Logger.Trace.LogInfo(String.Format("<date-time date=\"{0}\"><hour>{1}</hour><min>{2}</min><sec>{3}</sec></date-time>",
                        dt.Date.ToString("yyyy-MM-dd"), dt.Hour, dt.Minute, dt.Second));
                }
            }
            catch (ThreadAbortException)
            {
            }
            finally
            {
                this.mreStopPending.Set();
            }
        }

        protected override void OnStop(OnStopServiceArgs args)
        {
            this.timer.Abort();
            this.mreStopPending.WaitOne();
        }
    }
}
