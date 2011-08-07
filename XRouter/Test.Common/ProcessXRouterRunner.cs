using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace XRouter.Test.Common
{
    class ProcessXRouterRunner : DaemonNtDebugXRouterRunner, IDisposable
    {
        Process daemonProcess;

        public static readonly string DefaultExecutablePath =
            @"..\..\..\ComponentHosting\bin\debug\DaemonNT.exe";

        public string ExecutablePath { get; set; }

        public ProcessXRouterRunner(string daemonNtConfigFile, string serviceName)
            : base(daemonNtConfigFile, serviceName)
        {
            ExecutablePath = DefaultExecutablePath;
        }

        public override void Start()
        {
            string binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string commands = Path.Combine(binPath, ExecutablePath);
            string arguments = string.Format("--config-file={0} debug {1}", DaemonNtConfigFile, ServiceName);
            var daemonProcessInfo = new ProcessStartInfo(commands, arguments);
            daemonProcessInfo.WorkingDirectory = Path.GetDirectoryName(commands);
            daemonProcessInfo.RedirectStandardInput = true;
            daemonProcessInfo.UseShellExecute = false;
            daemonProcess = Process.Start(daemonProcessInfo);

            System.Threading.Thread.Sleep(1000);
        }

        public override void Stop()
        {
            daemonProcess.StandardInput.WriteLine("exit");
            if (!daemonProcess.WaitForExit(2000))
            {
                daemonProcess.Kill();
                daemonProcess.WaitForExit(2000);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (daemonProcess != null)
            {
                daemonProcess.Dispose();
            }
        }

        #endregion
    }
}
