using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaemonNT;
using XRouter.Common;
using XRouter.Broker;
using System.Reflection;
using System.IO;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common.MessageFlowConfig;
using XRouter.Processor.BuiltInActions;
using System.Xml.Linq;
using XRouter.Common.Xrm;

namespace XRouter.ComponentHosting
{
    class AllInOneXRouterHost : Service
    {
        private static readonly string SectionKey_Broker = "broker";
        private static readonly string SectionKey_Gateway = "gateway";
        private static readonly string SectionKey_Processor = "processor";
        private static readonly string SettingsKey_ComponentName = "component-name";

        private IBrokerService broker;
        private IGatewayService gateway;
        private IProcessorService processor;

        protected override void OnStart(OnStartServiceArgs args)
        {
            TraceLog.Initialize(Logger);
            EventLog.Initialize(Logger);

            broker = new XRouter.Broker.BrokerService();
            processor = new XRouter.Processor.ProcessorService();
            gateway = new XRouter.Gateway.Gateway();

            string processorName = args.Settings[SectionKey_Processor].Parameters[SettingsKey_ComponentName];
            string gatewayName = args.Settings[SectionKey_Gateway].Parameters[SettingsKey_ComponentName];

            broker.Start(
                new[] { new GatewayProvider(gatewayName, gateway) },
                new[] { new ProcessorProvider(processorName, processor) }
            );

            processor.Start(processorName, broker);
            gateway.Start(gatewayName, broker);



            //var config = broker.GetConfiguration();

            //#region Create message flow
            //var sendToA_Action = new ActionConfiguration() {
            //    PluginTypeFullName = typeof(SendMessageAction).FullName,
            //    PluginConfiguration = new SerializableXDocument(XDocument.Parse("<target>A</target>"))
            //};
            //var sendToB_Action = new ActionConfiguration() {
            //    PluginTypeFullName = typeof(SendMessageAction).FullName,
            //    PluginConfiguration = new SerializableXDocument(XDocument.Parse("<target>B</target>"))
            //};
            //var sendToC_Action = new ActionConfiguration() {
            //    PluginTypeFullName = typeof(SendMessageAction).FullName,
            //    PluginConfiguration = new SerializableXDocument(XDocument.Parse("<target>C</target>"))
            //};
            //var inputMessageSelection = new TokenSelection("/token/messages/message[@name='input']/*[1]");

            //var terminate_Node = new TerminatorNodeConfiguration() { Name = "term1", IsReturningOutput = false };

            //var sendToC_Node = new ActionNodeConfiguration() { Name = "sendToC", NextNode = terminate_Node, Actions = { sendToC_Action } };
            //var switchC_Node = new CbrNodeConfiguration() { Name = "switchC", TestedSelection = inputMessageSelection, DefaultTarget = terminate_Node };
            //switchC_Node.Branches.Add(new XrmUri("//item[@name='containsC']"), sendToC_Node);

            //var sendToB_Node = new ActionNodeConfiguration() { Name = "sendToB", NextNode = switchC_Node, Actions = { sendToB_Action } };
            //var switchB_Node = new CbrNodeConfiguration() { Name = "switchB", TestedSelection = inputMessageSelection, DefaultTarget = switchC_Node };
            //switchB_Node.Branches.Add(new XrmUri("//item[@name='containsB']"), sendToB_Node);

            //var sendToA_Node = new ActionNodeConfiguration() { Name = "sendToA", NextNode = switchB_Node, Actions = { sendToA_Action } };
            //var switchA_Node = new CbrNodeConfiguration() { Name = "switchA", TestedSelection = inputMessageSelection, DefaultTarget = switchB_Node };
            //switchA_Node.Branches.Add(new XrmUri("//item[@name='containsA']"), sendToA_Node);

            //var messageFlowConfig = new MessageFlowConfiguration("messageflow1", 1) {
            //    Nodes = { switchA_Node, switchB_Node, switchC_Node, sendToA_Node, sendToB_Node, sendToC_Node, terminate_Node },
            //    RootNode = switchA_Node
            //};
            //#endregion

            //config.AddMessageFlow(messageFlowConfig);
            //config.SetCurrentMessageFlowGuid(messageFlowConfig.Guid);
            //broker.ChangeConfiguration(config);
        }

        protected override void OnStop(OnStopServiceArgs args)
        {
            gateway.Stop();
            processor.Stop();
            broker.Stop();
        }
    }
}
