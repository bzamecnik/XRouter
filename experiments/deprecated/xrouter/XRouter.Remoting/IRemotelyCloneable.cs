using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Remoting
{
    public interface IRemotelyCloneable
    {
        string SerializeClone();

        void DeserializeClone(string serialized);
    }
}
