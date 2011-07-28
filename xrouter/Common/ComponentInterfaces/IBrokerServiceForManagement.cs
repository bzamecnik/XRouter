using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace XRouter.Common.ComponentInterfaces
{
    [ServiceContract]
    public interface IBrokerServiceForManagement
    {
        // NOTE: there should not be a single property instead of the following
        // two methods
        // - these methods can execute slowly, non-locally and with side-effects

        [OperationContract]
        ApplicationConfiguration GetConfiguration();

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
