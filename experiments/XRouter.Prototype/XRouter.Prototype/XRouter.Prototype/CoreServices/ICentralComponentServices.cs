using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Prototype.CoreTypes;
using System.Xml.Linq;

namespace XRouter.Prototype.CoreServices
{
    interface ICentralComponentServices
    {
        void ReceiveToken(Token token);

        void UpdateTokenWorkflowState(Guid tokenGuid, Token token);
        void AddMessageToToken(Guid tokenGuid, XDocument message);

        void RegisterOutputEndPoint(EndPointAddress address, IOutputEndpoint outputEndPoint);
        void UnregisterOutputEndPoint(EndPointAddress address);
        void SendMessageToOutputEndPoint(EndPointAddress address, Token messageToken, Action<Token> resultCallback);
    }
}
