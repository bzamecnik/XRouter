using System;
using System.Xml.Linq;

namespace XRouter.Gateway
{
    public delegate void MessageResultHandler(Guid tokenGuid, XDocument resultMessage, XDocument sourceMetadata, object context);

	public interface IAdapterService
	{
        void ReceiveMessage(XDocument message, string endpointName, XDocument metadata, object context, MessageResultHandler resultHandler);
	}
}
