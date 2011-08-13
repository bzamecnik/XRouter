using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ObjectRemoter.BasicSystemTest.Server
{
    /// <summary>
    /// Automatic message flood generator.
    /// </summary>
    class RandomizedFloodServer : IConsoleServer
    {
        private Random random = new Random();
        private StringBuilder stringBuilder = new StringBuilder();

        public event LineEnterHandler LineEntered = delegate { };

        /// <summary>
        /// Minimum random message length (inclusive).
        /// </summary>
        public int MinMessageLength = 10;
        /// <summary>
        /// Maximum random message length (inclusive).
        /// </summary>
        public int MaxMessageLength = 70;
        /// <summary>
        /// Interval between messages (in milliseconds).
        /// </summary>
        public int IntervalMillis = 10;

        public void WriteLine(string text)
        {
            //Console.WriteLine(text);
        }

        public void Start()
        {
            Console.WriteLine("Generating random messages with length [{0}; {1}]",
                MinMessageLength, MaxMessageLength);
            Console.WriteLine("Press CTRL+C to stop the program.");
            while (true)
            {
                string message = GenerateMessage();
                Console.WriteLine(message);
                LineEntered(message);
                Thread.Sleep(IntervalMillis);
            }
        }

        public string GenerateMessage()
        {
            int length = random.Next(MinMessageLength, MaxMessageLength);
            stringBuilder.Clear();
            for (int i = 0; i < length; i++)
            {
                int randomChar = random.Next(32, 126);
                stringBuilder.Append((char)randomChar);
            }
            string randomString = stringBuilder.ToString();
            stringBuilder.Clear();
            return randomString;
        }
    }
}
