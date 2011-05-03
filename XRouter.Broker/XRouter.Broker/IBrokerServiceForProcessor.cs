using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectRemoter;
using XRouter.Common;

namespace XRouter.Broker
{
    public interface IBrokerServiceForProcessor : IBrokerServiceForComponent
    {
        void UpdateTokenWorkflowState(Guid tokenGuid, WorkflowState workflowState);
        void AddMessageToToken(Guid tokenGuid, SerializableXDocument message);
        void FinishToken(Guid tokenGuid);

        SerializableXDocument SendMessageToOutputEndPoint(EndPointAddress address, SerializableXDocument message);
    }
}
