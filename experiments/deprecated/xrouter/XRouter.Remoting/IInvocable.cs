using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Remoting
{
    public interface IInvocable : IRemotelyReferable
    {
        object Invoke(object[] arguments);
    }
}
