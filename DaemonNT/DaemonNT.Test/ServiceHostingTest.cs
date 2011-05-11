using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using DaemonNT;
using Xunit;

namespace DaemonNT.Test
{
    public class ServiceHostingTest
    {
        // TODO:
        // - test the following:
        //   - installing an NT service
        //     - if it is not installed -> good
        //     - if it is already installed -> bad
        //   - uninstalling an NT service
        //     - if it is already installed -> good
        //     - if it is not installed -> bad
        //   - running an NT service
        //     - if it is already installed -> good
        //     - if it is not installed -> bad
        //   - running a service in debug mode

        /// <summary>
        /// Installs a service.
        /// </summary>
        /// <remarks>
        /// It assumes that the service is not installed yet.
        /// </remarks>
        [Fact, Trait("Category", "RunAsAdmin")]
        public void InstallNotAlreadyInstalledService()
        {
            string serviceName = "MyServer";
            ServiceController sc = new ServiceController(serviceName);

            ServiceCommands commands = new ServiceCommands();
            commands.Install(serviceName);

            // verify that the service has been installed
            string status = commands.GetStatus(serviceName);
            Assert.Equal(ServiceControllerStatus.Stopped.ToString(), status);
        }

        /// <summary>
        /// Uninstalls a service.
        /// </summary>
        /// <remarks>
        /// It assumes that the service is installed yet.
        /// </remarks>
        [Fact, Trait("Category", "RunAsAdmin")]
        public void UninstallAlreadyInstalledService()
        {
            string serviceName = "MyServer";
            ServiceController sc = new ServiceController(serviceName);

            ServiceCommands commands = new ServiceCommands();
            commands.Install(serviceName);

            // verify that the service has been uninstalled
            // TODO: checking empty status is not the best way
            string status = commands.GetStatus(serviceName);
            Assert.Equal(string.Empty, status);
        }

        /// <summary>
        /// Starts and then stops a service.
        /// </summary>
        /// <remarks>
        /// It assumes that the service is installed and stopped.
        /// </remarks>
        [Fact, Trait("Category", "RunAsAdmin")]
        public void StartAndStopInstalledService()
        {
            string serviceName = "MyServer";
            TimeSpan timeout = new TimeSpan(0, 0, 0, 2); // 2 sec
            ServiceCommands commands = new ServiceCommands();

            ServiceControllerStatus runningStatus = ServiceControllerStatus.Running;
            ServiceControllerStatus stoppedStatus = ServiceControllerStatus.Stopped;

            if (!stoppedStatus.ToString().Equals(
                commands.GetStatus(serviceName)))
            {
                Assert.True(false, "The service is not prepared to run.");
            }

            using (ServiceController sc = new ServiceController(serviceName))
            {
                // execute
                commands.Start(serviceName);

                // verify 
                sc.WaitForStatus(runningStatus, timeout);
                Assert.Equal(runningStatus.ToString(), commands.GetStatus(serviceName));

                // execute
                commands.Stop(serviceName);

                // verify
                sc.WaitForStatus(stoppedStatus, timeout);
                Assert.Equal(stoppedStatus.ToString(), commands.GetStatus(serviceName));
            }
        }

        //[Fact]
        public void GetServiceStatus()
        {
            string serviceName = "MyServer";
            string status = new ServiceCommands().GetStatus(serviceName);
            Console.WriteLine("{0}: {1}", serviceName, status);
        }
    }
}
