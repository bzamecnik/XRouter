using System;
using System.ServiceModel;

namespace XRouter.Common.ComponentInterfaces
{
    /// <summary>
    /// API of a broker component to be used for management of the XRouter
    /// application. It can be over over a WCF web service.
    /// </summary>
    [ServiceContract]
    public interface IBrokerServiceForManagement
    {
        /// <summary>
        /// Obtains the complete XRouter application configuration.
        /// </summary>
        /// <remarks>
        /// NOTE: There should not be a single property instead of the methods
        /// GetConfiguration() and ChangeConfiguration(). These methods can
        /// execute slowly, non-locally and with side-effects.
        /// </remarks>
        /// <returns>current configuration</returns>
        [OperationContract]
        ApplicationConfiguration GetConfiguration();

        /// <summary>
        /// Updates the XRouter application configuration.
        /// </summary>
        /// <param name="config">new application configuration</param>
        [OperationContract]
        void ChangeConfiguration(ApplicationConfiguration config);

        [OperationContract]
        EventLogEntry[] GetEventLogEntries(
            DateTime minDate,
            DateTime maxDate,
            LogLevelFilters logLevelFilter,
            int pageSize,
            int pageNumber);

        [OperationContract]
        TraceLogEntry[] GetTraceLogEntries(
            DateTime minDate,
            DateTime maxDate,
            LogLevelFilters logLevelFilter,
            int pageSize,
            int pageNumber);

        [OperationContract]
        Token[] GetTokens(int pageSize, int pageNumber);
    }
}
