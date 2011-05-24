using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Xrm;
using XRouter.Common;

namespace XRouter.Broker
{
    // methods to be called by a processor
    public interface IBrokerServiceForProcessor : IBrokerServiceForComponent
    {
        void UpdateTokenMessageFlowState(string updatingProcessorName, Guid tokenGuid, MessageFlowState messageFlowState);

        // NOTE: messages will not be removed from tokens

        void AddMessageToToken(string updatingProcessorName, Guid targetTokenGuid, string messageName, SerializableXDocument message);

        /// change token state, not messageFlow state
        void FinishToken(string updatingProcessorName, Guid tokenGuid, SerializableXDocument resultMessage);

        // NOTE: synchronous
        SerializableXDocument SendMessage(EndpointAddress address, SerializableXDocument message);

        MessageFlowConfiguration[] GetActiveMessageFlows();

        SerializableXDocument GetXmlResource(XrmTarget target);
    }
}
