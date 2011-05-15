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
        void UpdateTokenWorkflowState(string updatingProcessorName, Guid tokenGuid, WorkflowState workflowState);

        // NOTE: messages will not be removed from tokens

        void AddMessageToToken(string updatingProcessorName, Guid tokenGuid, SerializableXDocument message);

        /// change token state, not workflow state
        void FinishToken(string updatingProcessorName, Guid tokenGuid, SerializableXDocument resultMessage);

        // NOTE: synchronous
        SerializableXDocument SendMessageToOutputEndPoint(EndpointAddress address, SerializableXDocument message);
    }
}
