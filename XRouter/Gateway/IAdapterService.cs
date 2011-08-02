using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Common;

namespace XRouter.Gateway
{
    public delegate void MessageResultHandler(Guid tokenGuid, XDocument resultMessage, XDocument sourceMetadata, object context);

	public interface IAdapterService
	{
        void ReceiveMessage(XDocument message, string endpointName, XDocument metadata, object context, MessageResultHandler resultHandler);
	}
}
