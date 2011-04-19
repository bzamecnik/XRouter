using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectRemoter.RemoteCommunication
{
    /// <summary>
    /// Handler of a request.
    /// </summary>
    /// <param name="command">A command name to handle.</param>
    /// <param name="data">Additional request data.</param>
    /// <returns>Response</returns>
    delegate string RequestHandler(string command, string[] data);

    /// <summary>
    /// A server of a remote communication.
    /// </summary>
    interface IServer
    {
        /// <summary>
        /// Address of this server.
        /// </summary>
        ServerAddress Address { get; }

        /// <summary>
        /// Allows to add handler of received requests.
        /// </summary>
        event RequestHandler RequestReceived;

        /// <summary>
        /// Starts server.
        /// </summary>
        void Start();
    }
}
