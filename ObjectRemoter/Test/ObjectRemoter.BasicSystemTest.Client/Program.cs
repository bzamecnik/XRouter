using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ObjectRemoter.BasicSystemTest.Server;
using System.Threading;

namespace ObjectRemoter.BasicSystemTest.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[This is CLIENT]");
            Console.WriteLine();

            #region Using remote service
            Uri serviceUri = new Uri(File.ReadAllText(@"..\..\..\service_address.txt"));
            IConsoleService service = ServiceRemoter.GetServiceProxy<IConsoleService>(serviceUri);
            service.WriteLine("This text was send from a client to a service.");
            service.WriteLine(new Token("token1"));
            #endregion


            #region Using remote object
            // Read remote object address from file
            string serializedRemoteAddress = File.ReadAllText(@"..\..\..\object_address.txt");
            RemoteObjectAddress remoteAddress = RemoteObjectAddress.Deserialize(serializedRemoteAddress);

            // Create proxy for remote object
            IConsoleServer consoleServer = RemoteObjectProxyProvider.GetProxy<IConsoleServer>(remoteAddress);

            // Call this method on remote object
            consoleServer.WriteLine("This text comes from client but it should appear on a server.");

            // Register on remote object to its events
            consoleServer.LineEntered += consoleServer_LineEntered;

            // Keep application running so that we can continue receive events from remote object
            while (true)
            {
                Thread.Sleep(50);
            }
            #endregion
        }

        static void consoleServer_LineEntered(string line)
        {
            Console.WriteLine("This text was entered on server: " + line);
        }
    }
}
