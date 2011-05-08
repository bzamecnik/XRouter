using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Gateway.Implementation;
using XRouter.Common;

namespace XRouter.Gateway
{
	public interface IAdapterService
	{
		//IInputEndpoint CreateInputEndpoint(string endpointName, Action<ReceivedMessageHandler> registerListener);

		//IOutputEndpoint CreateOutputEndpoint(string endpointName, Action<Token, MessageSendResultHandler> sendAsync);

        XDocument ReceiveMessage(Token token);
		void ReceiveMessageAsync(Token token);
	}
}
