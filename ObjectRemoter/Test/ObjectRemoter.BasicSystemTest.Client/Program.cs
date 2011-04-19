using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ObjectRemoter.BasicSystemTest.Server;

namespace ObjectRemoter.BasicSystemTest.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[This is CLIENT]");
            Console.WriteLine();

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
            while (true) ;
        }

        static void consoleServer_LineEntered(string line)
        {
            Console.WriteLine("This text was entered on server: " + line);
        }
    }
}
