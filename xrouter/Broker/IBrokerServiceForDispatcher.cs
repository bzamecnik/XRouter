using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;

namespace XRouter.Broker
{
    internal interface IBrokerServiceForDispatcher
    {
        ApplicationConfiguration GetConfiguration();

        IEnumerable<ProcessorAccessor> GetProcessors();

        IEnumerable<Token> GetActiveTokensAssignedToProcessor(string processorName);
        IEnumerable<Token> GetUndispatchedTokens();

        void UpdateTokenAssignedProcessor(Guid tokenGuid, string assignedProcessor);
        void UpdateTokenMessageFlow(Guid tokenGuid, Guid messageFlowGuid);
        void UpdateTokenLastResponseFromProcessor(Guid tokenGuid, DateTime lastResponse);
        Token GetToken(Guid tokenGuid);
    }
}
