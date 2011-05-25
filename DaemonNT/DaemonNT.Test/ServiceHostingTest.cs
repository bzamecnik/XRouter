namespace DaemonNT.Test
{
    using System;
    using System.ServiceProcess;
    using DaemonNT;
    using DaemonNT.Installation;
    using Xunit;

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
        /// Installs a service and verifies it has been installed.
        /// </summary>
        /// <remarks>
        /// It assumes that the service is not installed yet.
        /// WARNING: as the service is installed the command to run the service
        /// is created based on the currect appdomain base directory which
        /// depends on from where the test is ran.
        /// </remarks>
        [Fact, Trait("Category", "RunAsAdmin")]
        public void InstallNotAlreadyInstalledService()
        {
            string serviceName = "MyServer";

            // assume the service is not installed or the test can't work
            Assert.False(InstallerServices.IsInstalled(serviceName), "The service must not be installed.");

            ServiceCommands commands = new ServiceCommands();
            bool installed = commands.Install(serviceName);

            // verify that the service has been installed
            Assert.True(installed, "The service has not been installed.");
            Assert.True(InstallerServices.IsInstalled(serviceName), "The service is still not installed.");

            // installed service should be stopped by default
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

            Assert.True(InstallerServices.IsInstalled(serviceName), "The service must be installed.");

            ServiceCommands commands = new ServiceCommands();
            bool uninstalled = commands.Uninstall(serviceName);

            // verify that the service has been uninstalled
            Assert.True(uninstalled, "The service has not been uninstalled.");
            Assert.False(InstallerServices.IsInstalled(serviceName), "The service is still installed.");
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
            TimeSpan timeout = new TimeSpan(0, 0, 0, 1); // 1 sec
            ServiceCommands commands = new ServiceCommands();

            ServiceControllerStatus runningStatus = ServiceControllerStatus.Running;
            ServiceControllerStatus stoppedStatus = ServiceControllerStatus.Stopped;

            if (!stoppedStatus.ToString().Equals(
                commands.GetStatus(serviceName)))
            {
                // there's no Assert.Equal with a user message
                Assert.True(false, "The service is not prepared to run.");
            }

            using (ServiceController sc = new ServiceController(serviceName))
            {
                // execute
                bool started = commands.Start(serviceName);

                // verify 
                Assert.True(started, "The service was not started.");
                sc.WaitForStatus(runningStatus, timeout);
                Assert.Equal(runningStatus.ToString(), commands.GetStatus(serviceName));

                // execute
                bool stopped = commands.Stop(serviceName);

                // verify
                Assert.True(stopped, "The service was not stopped.");
                sc.WaitForStatus(stoppedStatus, timeout);
                Assert.Equal(stoppedStatus.ToString(), commands.GetStatus(serviceName));
            }
        }

        //[Fact]
        //public void StartServiceInDebugMode()
        //{
        //    string serviceName = "MyServer";
        //    ServiceCommands commands = new ServiceCommands();
        //    commands.Debug(serviceName);
        //}
    }
}
