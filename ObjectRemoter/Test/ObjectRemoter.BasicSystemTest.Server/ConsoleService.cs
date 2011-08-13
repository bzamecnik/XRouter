using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectRemoter.BasicSystemTest.Server
{
    class ConsoleService : IConsoleService
    {
        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        public void WriteLine(Token token)
        {
            Console.WriteLine("Received token: " + token.ToString());
        }
    }
}
