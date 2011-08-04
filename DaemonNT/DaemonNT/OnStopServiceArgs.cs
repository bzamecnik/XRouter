using System;

namespace DaemonNT
{
    /// <summary>
    /// Arguments for stopping the service with the Service.OnStop() method.
    /// </summary>
    /// <seealso cref="Service"/>
    public sealed class OnStopServiceArgs
    {
        /// <summary>
        /// Specifies whether the service is being stopped because the
        /// operating system is being stopped.
        /// </summary>
        public Boolean Shutdown { internal set; get; }

        internal OnStopServiceArgs()
        { }
    }
}
