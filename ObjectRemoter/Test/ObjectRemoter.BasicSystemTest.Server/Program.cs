using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ObjectRemoter.BasicSystemTest.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[This is SERVER]");
            Console.WriteLine();

            #region Providing a service
            // Create and publish a service
            Uri serviceUri = ServiceRemoter.PublishService<IConsoleService>(new ConsoleService());
            File.WriteAllText(@"..\..\..\service_address.txt", serviceUri.ToString());
            #endregion


            #region Providing an object
            // Create real local object
            ConsoleServer consoleServer = new ConsoleServer();
            //RandomizedFloodServer consoleServer = new RandomizedFloodServer();

            // Publish object to be accessible remotely
            RemoteObjectAddress address = ObjectServer.PublishObject(consoleServer);

            // Write remote object address into file so that a client can read it.
            string serializedAddress = address.Serialize();
            File.WriteAllText(@"..\..\..\object_address.txt", serializedAddress);

            Console.WriteLine("When you enter a line with text, an event will be raised on a client.");
            Console.WriteLine();
            consoleServer.Start();
            #endregion
        }
    }
}
