using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using DaemonNT;

namespace DaemonNT.Test
{
    // Implementace trivialni sluzby, ktera dle daneho intervalu zapise do trace-logu.

    public class TestService : DaemonNT.Service
    {
        private Timer timer = null;

        private DaemonNT.Logging.Logger logger = null;

        protected override void OnStart(ServiceArgs args)
        {
            this.logger = args.Logger;

            Int32 interval = Convert.ToInt32(args.Setting["timer"].Param["interval"]);

            // incializuje timer
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
            this.timer.Enabled = true;
        }

        protected override void OnStop(Boolean shutdown)
        {
            this.logger.Event.LogInfo("Service stopped!");
        }
    }
}
