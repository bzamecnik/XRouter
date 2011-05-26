namespace DaemonNT.Test
{
    using System;
    using System.ServiceProcess;
    using DaemonNT;
    using DaemonNT.Installation;
    using Xunit;
    using Xunit.Extensions;

    /// <summary>
    /// This is a whole scenario of installing, running, and uninstalling a
    /// service.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It works with external state so the parts must be ordered and depend
    /// one on another. The input assumption is that the test service is not
    /// installed. The output assumption is the same - if the scenario went
    /// alright the service will not be installed.
    /// </para>
    /// <para>
    /// NOTE: Tunning the tests requires Administrator privileges!
    /// </para>
    /// <para>
    /// The scenario is:
    /// Install a not installed service (good).
    /// Try to install the already installed service (bad).
    /// Start and stop the installed service (good).
    /// Uninstall the installed service (good).
    /// Try to install a not installed service (bad).
    /// Try to start a not installed service (bad).
    /// </para>
    /// </remarks>
    [PrioritizedFixture]
    public class ServiceHostingTest
    {
        /// <summary>
        /// Installs a service and verifies it has been installed.
        /// </summary>
        /// <remarks>
        /// It assumes that the service is not installed yet.
        /// WARNING: as the service is installed the command to run the service
        /// is created based on the currect appdomain base directory which
        /// depends on from where the test is ran.
        /// </remarks>
        [Fact]
        [Trait("Category", "RunAsAdmin")]
        [TestPriority(1)]
        public void InstallNotAlreadyInstalledService()
        {
            string serviceName = "MyServer";

            // assume the service is NOT installed or the test can't work
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

        [Fact]
        [Trait("Category", "RunAsAdmin")]
        [TestPriority(2)]
        public void InstallAlreadyInstalledService()
        {
            string serviceName = "MyServer";

            // assume the service IS installed or the test can't work
            Assert.True(InstallerServices.IsInstalled(serviceName), "The service must be installed.");

            ServiceCommands commands = new ServiceCommands();
            bool installed = commands.Install(serviceName);

            // verify that the installation failed
            Assert.False(installed, "The service installer did not fail.");
            // verify that the service is still installed from the previous instalation
            Assert.True(InstallerServices.IsInstalled(serviceName), "The service is still not installed.");
        }


        /// <summary>
        /// Starts and then stops a service.
        /// </summary>
        /// <remarks>
        /// It assumes that the service is installed and stopped.
        /// </remarks>
        [Fact]
        [Trait("Category", "RunAsAdmin")]
        [TestPriority(3)]
        public void StartAndStopInstalledService()
        {
            string serviceName = "MyServer";

            Assert.True(InstallerServices.IsInstalled(serviceName), "The service must be installed.");

            TimeSpan timeout = new TimeSpan(0, 0, 0, 1); // 1 sec
            ServiceCommands commands = new ServiceCommands();

            ServiceControllerStatus runningStatus = ServiceControllerStatus.Running;
            ServiceControllerStatus stoppedStatus = ServiceControllerStatus.Stopped;

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

        /// <summary>
        /// Uninstalls a service.
        /// </summary>
        /// <remarks>
        /// It assumes that the service is installed yet.
        /// </remarks>
        [Fact]
        [Trait("Category", "RunAsAdmin")]
        [TestPriority(4)]
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

        [Fact]
        [Trait("Category", "RunAsAdmin")]
        [TestPriority(5)]
        public void UninstallNotInstalledService()
        {
            string serviceName = "MyServer";

            Assert.False(InstallerServices.IsInstalled(serviceName), "The service must not be installed.");

            ServiceCommands commands = new ServiceCommands();
            bool uninstalled = commands.Uninstall(serviceName);

            // verify that the service has NOT been tried to be uninstalled
            Assert.False(uninstalled, "The service uninstaller failed to recognize a not installed service.");
        }

        [Fact]
        [Trait("Category", "RunAsAdmin")]
        [TestPriority(6)]
        public void StartNotInstalledService()
        {
            string serviceName = "MyServer";

            Assert.False(InstallerServices.IsInstalled(serviceName), "The service must not be installed.");

            ServiceCommands commands = new ServiceCommands();
            bool started = commands.Start(serviceName);

            Assert.False(started, "The service was started even if it was not installed.");
        }
    }
}
