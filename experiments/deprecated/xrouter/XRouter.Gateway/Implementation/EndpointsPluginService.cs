using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Management;
using XRouter.Remoting;
using System.Collections.ObjectModel;

namespace XRouter.Gateway.Implementation
{
    class EndpointsPluginService : IEndpointsPluginService
    {
        internal Gateway Gateway { get; private set; }

        internal string PluginName { get; private set; }

        private List<InputEndpoint> InternalInputEndpoints { get; set; }
        public ReadOnlyCollection<InputEndpoint> InputEndpoints { get; set; }

        private List<OutputEndpoint> InternalOutputEndpoints { get; set; }
        public ReadOnlyCollection<OutputEndpoint> OutputEndpoints { get; set; }

        internal IEndpointsPlugin Client { get; set; }

        public EndpointsPluginService(Gateway gateway, string pluginName)
        {
            Gateway = gateway;
            PluginName = pluginName;

            InternalInputEndpoints = new List<InputEndpoint>();
            InternalOutputEndpoints = new List<OutputEndpoint>();
            InputEndpoints = new ReadOnlyCollection<InputEndpoint>(InternalInputEndpoints);
            OutputEndpoints = new ReadOnlyCollection<OutputEndpoint>(InternalOutputEndpoints);
        }

        public IInputEndpoint CreateInputEndpoint(string endpointName, Action<ReceivedMessageHandler> registerListener)
        {
            var address = new EndpointAddress(Gateway.Name, PluginName, endpointName);
            var result = new InputEndpoint(address);
            registerListener(delegate (Message message) {
                result.ReceiveMessage(message); 
            });
            InternalInputEndpoints.Add(result);
            return result;
        }

        public IOutputEndpoint CreateOutputEndpoint(string endpointName, Action<Message, MessageSendResultHandler> sendAsync)
        {
            var address = new EndpointAddress(Gateway.Name, PluginName, endpointName);
            var result = new OutputEndpoint(address, sendAsync);
            InternalOutputEndpoints.Add(result);
            return result;
        }
    }
}
