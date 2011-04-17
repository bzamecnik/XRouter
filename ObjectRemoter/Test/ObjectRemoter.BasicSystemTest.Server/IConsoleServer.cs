using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectRemoter.BasicSystemTest.Server
{
    public delegate void LineEnterHandler(string line);

    public interface IConsoleServer : IRemotelyReferable
    {
        /// <summary>
        /// Raises when user enters a line with text.
        /// </summary>
        event LineEnterHandler LineEntered;

        /// <summary>
        /// Writes a text on a console.
        /// </summary>
        /// <param name="text"></param>
        void WriteLine(string text);
    }
}
