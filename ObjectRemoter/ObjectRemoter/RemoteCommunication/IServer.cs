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
    internal delegate string RequestHandler(string command, string[] data);

    /// <summary>
    /// A server of a remote communication.
    /// </summary>
    internal interface IServer
    {
        /// <summary>
        /// Allows to add handler of received requests.
        /// </summary>
        event RequestHandler RequestReceived;

        /// <summary>
        /// Address of this server.
        /// </summary>
        ServerAddress Address { get; }

        /// <summary>
        /// Starts server.
        /// </summary>
        void Start();
    }
}
