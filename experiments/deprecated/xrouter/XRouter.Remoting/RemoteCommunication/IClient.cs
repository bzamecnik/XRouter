using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Remoting.RemoteCommunication
{
    public interface IClient
    {
        string Request(string command, string[] data);
    }
}
