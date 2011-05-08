using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Management.Implementation;

namespace XRouter.Management.Console.Implementation
{
    class ConsoleServer : IConsoleServer
    {
        public string Name { get { return "ConsoleServer"; } }

        private XRouterManager Manager { get; set; }

        public ConsoleServer(XRouterManager manager)
        {
            Manager = manager;
        }

        public void Initialize()
        {
        }

        public void ChangeConfiguration(RemotableXDocument configuration)
        {
            RemotableXElement rootElement = Manager.GetConfigData("/");
            Manager.NotifyConfigurationChanged(rootElement);
        }
    }
}
