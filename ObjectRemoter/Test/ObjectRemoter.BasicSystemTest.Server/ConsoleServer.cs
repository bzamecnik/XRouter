using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectRemoter.BasicSystemTest.Server
{
    /// <summary>
    /// A console which accepts text entered manually by a user.
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
            while (true)
            {
                string line = Console.ReadLine();
                LineEntered(line);
            }
        }
    }
}
