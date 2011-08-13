using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectRemoter
{
    /// <summary>
    /// A wrapper for a delegate which can be invoked remotely.
    /// </summary>
    public interface IInvocable : IRemotelyReferable
    {
        /// <summary>
        /// Starts the execution of the remote delegate.
        /// </summary>
        /// <param name="arguments"></param>
        /// <exception cref="ObjectRemoterException" />
        /// <returns>Return value of the delegate</returns>
        object Invoke(object[] arguments);
    }
}
