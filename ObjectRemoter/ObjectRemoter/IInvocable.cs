using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectRemoter
{
    /// <summary>
    /// A wrapper for delegate which can be invoked remotely.
    /// </summary>
    public interface IInvocable : IRemotelyReferable
    {
        object Invoke(object[] arguments);
    }
}
