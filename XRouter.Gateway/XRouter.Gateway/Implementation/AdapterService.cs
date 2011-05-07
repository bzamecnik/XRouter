using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using XRouter.Common;

namespace XRouter.Gateway.Implementation
{
	class AdapterService : IAdapterService
	{
		internal Gateway Gateway { get; private set; }

		internal string AdapterName { get; private set; }

        //private List<InputEndpoint> InternalInputEndpoints { get; set; }
        //public ReadOnlyCollection<InputEndpoint> InputEndpoints { get; private set; }

        //private List<OutputEndpoint> InternalOutputEndpoints { get; set; }
        //public ReadOnlyCollection<OutputEndpoint> OutputEndpoints { get; private set; }

		internal IAdapter Client { get; set; }

		public AdapterService(Gateway gateway, string pluginName)
		{
			Gateway = gateway;
			AdapterName = pluginName;

            //InternalInputEndpoints = new List<InputEndpoint>();
            //InternalOutputEndpoints = new List<OutputEndpoint>();
            //InputEndpoints = new ReadOnlyCollection<InputEndpoint>(InternalInputEndpoints);
            //OutputEndpoints = new ReadOnlyCollection<OutputEndpoint>(InternalOutputEndpoints);
		}

        //public IInputEndpoint CreateInputEndpoint(string endpointName, Action<ReceivedMessageHandler> registerListener)
        //{
        //    var address = new EndpointAddress(Gateway.Name, PluginName, endpointName);
        //    var result = new InputEndpoint(address);
        //    registerListener(delegate (Token token) {
        //        result.ReceiveMessage(token); 
        //    });
        //    InternalInputEndpoints.Add(result);
        //    return result;
        //}

        //public IOutputEndpoint CreateOutputEndpoint(string endpointName, Action<Token, MessageSendResultHandler> sendAsync)
        //{
        //    var address = new EndpointAddress(Gateway.Name, PluginName, endpointName);
        //    var result = new OutputEndpoint(address, sendAsync);
        //    InternalOutputEndpoints.Add(result);
        //    return result;
        //}

		public XDocument ReceiveMessage(Token token)
		{
            //! doplnit info do tokenu
            token.SourceAddress.GatewayName = Gateway.Name;
            return Gateway.ReceiveMessage(token);			
		}

		public void ReceiveMessageAsync(Token token)
		{
            //! doplnit info do tokenu
            token.SourceAddress.GatewayName = Gateway.Name;
            Gateway.ReceiveMessageAsync(token);
		}

        public SerializableXDocument SendMessageToOutputEndPoint(EndpointAddress address, SerializableXDocument message)
        {
            if (address.AdapterName != AdapterName) {
                throw new ArgumentException("Incorrect adapter name.", "address");
            }

            var result = Client.SendMessageToOutputEndPoint(address, message);
            return result;
        }
	}
}
