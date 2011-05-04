using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectRemoter;
using XRouter.Common;

namespace XRouter.Broker
{
    // methods to be called by a processor
    public interface IBrokerServiceForProcessor : IBrokerServiceForComponent
    {
        void UpdateTokenWorkflowState(Guid tokenGuid, WorkflowState workflowState);

        // NOTE: messages will not be removed from tokens

        void AddMessageToToken(Guid tokenGuid, SerializableXDocument message);

        /// change token state, not workflow state
        void FinishToken(Guid tokenGuid);

        // NOTE: synchronous
        SerializableXDocument SendMessageToOutputEndPoint(EndPointAddress address, SerializableXDocument message);
    }
}
