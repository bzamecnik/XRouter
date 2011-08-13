using System;
using System.ServiceModel;
using XRouter.Common;

namespace XRouter.Manager
{
    /// <summary>
    /// Specfikace rozhrani WCF, ktera poskytuje sluzby XRouter console (GUI).
    /// </summary>
    [ServiceContract]
    public interface IConsoleServer
    {
        [OperationContract]
        string GetXRouterServiceStatus();

        [OperationContract]
        void StartXRouterService(int timeout);

        [OperationContract]
        void StopXRouterService(int timeout);

        /// <summary>
        /// Obtains the complete application configuration of the managed
        /// XRouter service instance.
        /// </summary>
        /// <returns>current configuration</returns>
        [OperationContract]
        ApplicationConfiguration GetConfiguration();

        // TODO: rename to UpdateConfiguration() for match
        // XRouter.Common.ComponentInterfaces.IBrokerService

        /// <summary>
        /// Updates the complete application configuration of the managed
        /// XRouter service instance.
        /// </summary>
        [OperationContract]
        void ChangeConfiguration(ApplicationConfiguration config);

        [OperationContract]
        EventLogEntry[] GetEventLogEntries(DateTime minDate, DateTime maxDate, LogLevelFilters logLevelFilter, int pageSize, int pageNumber);

        [OperationContract]
        TraceLogEntry[] GetTraceLogEntries(DateTime minDate, DateTime maxDate, LogLevelFilters logLevelFilter, int pageSize, int pageNumber);

        [OperationContract]
        Token[] GetTokens(int pageSize, int pageNumber);
    }
}
