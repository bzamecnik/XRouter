using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectRemoter.BasicSystemTest.Server
{
    /// <summary>
    /// Internal implementation of console
    /// </summary>
    class ConsoleServer : IConsoleServer
    {
        public event LineEnterHandler LineEntered = delegate { };

        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        public void Start()
        {
            while (true) {
                string line = Console.ReadLine();
                LineEntered(line);
            }
        }
    }
}
