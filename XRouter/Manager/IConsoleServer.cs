using System;
using System.ServiceModel;
using XRouter.Common;

namespace XRouter.Manager
{
    /// <summary>
    /// API of a console server which provides services to the GUI (over a
    /// WCF web service).
    /// </summary>
    [ServiceContract]
    public interface IConsoleServer
    {
        /// <summary>
        /// Obtains the current status of the managed XRouterService run
        /// (running, stopped, etc.).
        /// </summary>
        /// <remarks>
        /// <para>
        /// In case the manager runs in DaemonNT debug mode, only Stopped is
        /// returned.
        /// </para>
        /// <para>
        /// Status format depends on the Windows Service controller.
        /// </para>
        /// </remarks>
        /// <returns></returns>
        [OperationContract]
        string GetXRouterServiceStatus();

        /// <summary>
        /// Starts an instance of the managed XRouterService and waits until
        /// it really runs.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A time-out results in an exception.
        /// </para>
        /// <para>
        /// In case the manager runs in DaemonNT debug mode this method
        /// has no effect.
        /// </para>
        /// </remarks>
        /// <param name="timeout">maximum time (in seconds) to wait for the
        /// 'running' status</param>
        [OperationContract]
        void StartXRouterService(int timeout);

        /// <summary>
        /// Stops a running instance of the managed XRouterService and waits
        /// until it is really stopped.
        /// </summary>
        /// <para>
        /// A time-out results in an exception.
        /// </para>
        /// <para>
        /// In case the manager runs in DaemonNT debug mode this method
        /// has no effect.
        /// </para>
        /// <param name="timeout">maximum time (in seconds) to wait for the
        /// 'stopped' status</param>
        [OperationContract]
        void StopXRouterService(int timeout);

        /// <summary>
        /// Obtains the complete application configuration of the managed
        /// XRouter service instance.
        /// </summary>
        /// <remarks>
        /// The configuration is taken from a persistent storage.
        /// </remarks>
        /// <returns>current configuration</returns>
        [OperationContract]
        ApplicationConfiguration GetConfiguration();

        // TODO: rename to UpdateConfiguration() for match
        // XRouter.Common.ComponentInterfaces.IBrokerService

        /// <summary>
        /// Updates the complete application configuration of the managed
        /// XRouter service instance.
        /// </summary>
        /// <remarks>
        /// <remarks>
        /// The configuration is stored to a persistent storage.
        /// </remarks>
        /// </remarks>
        [OperationContract]
        void ChangeConfiguration(ApplicationConfiguration config);

        /// <summary>
        /// Selects logged event log entries according to given criteria.
        /// </summary>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        /// <param name="logLevelFilter"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [OperationContract]
        EventLogEntry[] GetEventLogEntries(DateTime minDate, DateTime maxDate, LogLevelFilters logLevelFilter, int pageSize, int pageNumber);

        /// <summary>
        /// Selects logged trace log entries according to given criteria.
        /// </summary>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        /// <param name="logLevelFilter"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [OperationContract]
        TraceLogEntry[] GetTraceLogEntries(DateTime minDate, DateTime maxDate, LogLevelFilters logLevelFilter, int pageSize, int pageNumber);

        /// <summary>
        /// Selects tokens according to given criteria.
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [OperationContract]
        Token[] GetTokens(int pageSize, int pageNumber);
    }
}
