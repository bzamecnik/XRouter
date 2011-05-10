using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using DaemonNT;
using Xunit;

namespace DaemonNT.Test
{
    class ServiceHostingTest
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
        //
        // Check the NT service status with:
        //    using System.ServiceProcess;
        //    ServiceController sc = new ServiceController(SERVICENAME);
        //    switch (sc.Status) { ... }

        [Fact]
        public void InstallNotAlreadyInstalledService()
        {
            string serviceName = "MyServer";
            ServiceController sc = new ServiceController(serviceName);

            new ServiceCommands().Install(serviceName);

            try
            {
                ServiceControllerStatus status = sc.Status;
                Assert.Equal(ServiceControllerStatus.Stopped, status);
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void RunInstalledService()
        {
            string serviceName = "MyServer";
            ServiceController sc = new ServiceController(serviceName);

            Console.WriteLine(sc.Status);

            new ServiceCommands().Run(serviceName);


            Console.WriteLine(sc.Status);
        }

        [Fact]
        public void GetServiceStatus()
        {
            ServiceController sc = new ServiceController("MyServer");
            Console.WriteLine("MyServer: {0}", sc.Status);
        }
    }
}
