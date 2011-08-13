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
    /// The object can contain methods, properties and events. As for events,
    /// only their handlers can be remotely added or removed. Raising an event
    /// must be done locally on the remote host. It is possible to remotely
    /// call a method which raises the event inside itself. Event handlers
    /// added remotely are executed on the site where they have been added.
    /// </para>
    /// </remarks>
    public interface IRemotelyReferable
    {
    }
}
