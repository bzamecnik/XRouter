using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectRemoter.RemoteCommunication
{
    /// <summary>
    /// A client for remote communication.
    /// </summary>
    internal interface IClient
    {
        /// <summary>
        /// Send a request and receive response.
        /// </summary>
        /// <param name="command">Request command name</param>
        /// <param name="data">Request additional data</param>
        /// <returns>Response</returns>
        string Request(string command, string[] data);
    }
}
