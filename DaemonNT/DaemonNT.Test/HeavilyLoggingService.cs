namespace DaemonNT.Test
{
    using System;
    using System.Diagnostics;
    using DaemonNT;
    using System.Text;

    public class HeavilyLoggingService : Service
    {
        protected override void OnStart(OnStartServiceArgs args)
        {
            // event log
            this.Logger.Event.LogInfo(String.Format("ServiceName={0} IsDebugMode={1}",
                args.ServiceName, args.IsDebugMode));

            // get settings
            int messageCount = 0;
            int messageLength = 0;
            try
            {
                messageCount = Convert.ToInt32(args.Settings.Parameters["messageCount"]);
                messageLength = Convert.ToInt32(args.Settings.Parameters["messageLength"]);
            }
            catch (Exception e)
            {
                this.Logger.Event.LogError(String.Format("Settings are invalid! {0}", e.Message));
                throw e;
            }

            string message = GenerateMessage(messageLength);

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < messageCount; i++)
            {
                this.Logger.Trace.LogInfo(message);
            }

            stopwatch.Stop();

            this.Logger.Event.LogInfo(String.Format(
                "Logged {0} trace log events with messages of {1} bytes in {2} ms, average {3} ms/record",
                messageCount, messageLength, stopwatch.ElapsedMilliseconds,
                stopwatch.ElapsedMilliseconds / (float)messageCount));
        }

        private string GenerateMessage(int length)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                int randomChar = random.Next(64, 126);
                stringBuilder.Append((char)randomChar);
            }
            return stringBuilder.ToString();
        }
    }
}
