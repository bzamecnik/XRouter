using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectRemoter
{
    /// <summary>
    /// Objects implementing this interface will be remotely accessible via
    /// a remote proxy.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The interface is empty and only marks the object as it supports being
    /// remotely referred.
    /// </para>
    /// <para>
    /// The object can contain methods, properties and events.
    /// </para>
    /// </remarks>
    public interface IRemotelyReferable
    {
    }
}
