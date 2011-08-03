using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Test.Common
{
    public class TextGenerator
    {
        /// <summary>
        /// Minimum random message length (inclusive).
        /// </summary>
        public int MinMessageLength { get; set; }
        /// <summary>
        /// Maximum random message length (inclusive).
        /// </summary>
        public int MaxMessageLength { get; set; }

        private Random random = new Random();
        private StringBuilder stringBuilder = new StringBuilder();

        public TextGenerator()
        {
            MinMessageLength = 10;
            MaxMessageLength = 70;
        }

        public string GenerateMessage()
        {
            int length = random.Next(MinMessageLength, MaxMessageLength);
            stringBuilder.Clear();
            for (int i = 0; i < length; i++)
            {
                int randomChar = random.Next(65, 90);
                stringBuilder.Append((char)randomChar);
            }
            string randomString = stringBuilder.ToString();
            stringBuilder.Clear();
            return randomString;
        }
    }
}
