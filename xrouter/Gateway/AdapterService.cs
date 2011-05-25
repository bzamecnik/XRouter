using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using XRouter.Common;

namespace XRouter.Gateway
{
	class AdapterService : IAdapterService
	{
		internal Gateway Gateway { get; private set; }

		internal string AdapterName { get; private set; }

		internal IAdapter Adapter { get; set; }

		public AdapterService(Gateway gateway, string adapterName)
		{
			Gateway = gateway;
			AdapterName = adapterName;
		}

        public void ReceiveMessage(XDocument message, string endpointName, XDocument metadata = null,  MessageResultHandler resultHandler = null)
		{
            Token token = new Token();
            token.SaveSourceAddress(new EndpointAddress(Gateway.Name, AdapterName, endpointName));
            token.SaveSourceMetadata(metadata);
            token.AddMessage(Constants.InputMessageName, message);
            Gateway.ReceiveToken(token, resultHandler);
		}

        public XDocument SendMessage(EndpointAddress address, XDocument message, XDocument metadata)
        {
            if (address.AdapterName != AdapterName) {
                throw new ArgumentException("Incorrect adapter name.", "address");
            }

            XDocument result = Adapter.SendMessage(address.EndPointName, message, metadata);
            return result;
        }
	}
}
