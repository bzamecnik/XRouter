namespace DaemonNT.Test
{
    using System;
    using System.Threading;
    using DaemonNT;
    using DaemonNT.Logging;

    /// <summary>
    /// An example trivial service implementation. It shows how to start the
    /// service in a new thread, how to read settings and how to override the
    /// Service hook methods.
    /// </summary>
    /// <remarks>
    /// It reads input from console and answers back until it is stopped.
    /// </remarks>
    public class ExampleService : Service
    {
        string response;
        Thread thread;

        protected override void OnStart(OnStartServiceArgs args)
        {
            // event log
            this.Logger.Event.LogInfo(String.Format("ServiceName={0} IsDebugMode={1}",
                args.ServiceName, args.IsDebugMode));

            // get settings
            response = args.Settings["section"].Parameters["response"];

            // initialize and start the service thread
            thread = new Thread(new ThreadStart(this.ServiceLoop));
            thread.Start();
        }

        private void ServiceLoop()
        {
            try
            {
                while (true)
                {
                    string request = Console.ReadLine();
                    Console.WriteLine("Request: {0}, response: {1}", request, response);
                }
            }
            catch (ThreadAbortException ex)
            {
                Console.WriteLine("Service loop thread was aborted.");
            }
        }

        protected override void OnStop(OnStopServiceArgs args)
        {
            thread.Abort();
        }
    }
}
