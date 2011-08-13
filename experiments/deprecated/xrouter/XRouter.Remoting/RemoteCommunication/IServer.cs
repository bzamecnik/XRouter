using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Remoting.RemoteCommunication
{
    public delegate string RequestHandler(string command, string[] data);

    public interface IServer
    {
        ServerAddress Address { get; }

        event RequestHandler RequestArrived;

        void Start();
    }
}
