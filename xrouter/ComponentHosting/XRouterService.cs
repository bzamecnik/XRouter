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
using XRouter.Processor.MessageFlowBuilding;

namespace XRouter.ComponentHosting
{
    class XRouterService : Service
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

            ObjectConfigurator.Configurator.CustomItemTypes.Add(new TokenSelectionConfigurationItemType());

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

            var config = broker.GetConfiguration();

            #region Create message flow
            var terminator = MessageFlowBuilder.CreateTerminator("termination");

            #region Sample 1
            var sample1_sendToC = MessageFlowBuilder.CreateSender("sample1_sendToC", inputMessage: "input", outputEndpoint: "OutC", nextNode: terminator);
            var sample1_containsC = MessageFlowBuilder.CreateCbr("sample1_containsC", testedMessage: "input", defaultTarget: terminator, cases: new CbrCase("schematron_sample1_contains_C", sample1_sendToC));
            var sample1_sendToB = MessageFlowBuilder.CreateSender("sample1_sendToB", inputMessage: "input", outputEndpoint: "OutB", nextNode: sample1_containsC);
            var sample1_containsB = MessageFlowBuilder.CreateCbr("sample1_containsB", testedMessage: "input", defaultTarget: sample1_containsC, cases: new CbrCase("schematron_sample1_contains_B", sample1_sendToB));
            var sample1_sendToA = MessageFlowBuilder.CreateSender("sample1_sendToA", inputMessage: "input", outputEndpoint: "OutA", nextNode: sample1_containsB);
            var sample1_containsA = MessageFlowBuilder.CreateCbr("sample1_containsA", testedMessage: "input", defaultTarget: sample1_containsB, cases: new CbrCase("schematron_sample1_contains_A", sample1_sendToA));
            #endregion

            #region Sample 2
            var sample2_sendToA = MessageFlowBuilder.CreateSender("sample2_sendToA", inputMessage: "input", outputEndpoint: "OutA", nextNode: terminator);
            var sample2_sendToB = MessageFlowBuilder.CreateSender("sample2_sendToB", inputMessage: "input", outputEndpoint: "OutB", nextNode: terminator);
            var sample2_sendToC = MessageFlowBuilder.CreateSender("sample2_sendToC", inputMessage: "input", outputEndpoint: "OutC", nextNode: terminator);
            var sample2_priceSumSwitch = MessageFlowBuilder.CreateCbr("sample2_priceSumSwitch", testedMessage: "input", defaultTarget: terminator, cases: new[] {
                new CbrCase("schematron_sample2_targetA", sample2_sendToA),
                new CbrCase("schematron_sample2_targetB", sample2_sendToB),
                new CbrCase("schematron_sample2_targetC", sample2_sendToC)
            } );
            #endregion

            #region Sample 3
            var sample3_sendToA = MessageFlowBuilder.CreateSender("sample3_sendToA", inputMessage: "result", outputEndpoint: "OutA", nextNode: terminator);
            var sample3_sendToB = MessageFlowBuilder.CreateSender("sample3_sendToB", inputMessage: "result", outputEndpoint: "OutB", nextNode: terminator);
            var sample3_sendToC = MessageFlowBuilder.CreateSender("sample3_sendToC", inputMessage: "result", outputEndpoint: "OutC", nextNode: terminator);
            var sample3_targetSwitch = MessageFlowBuilder.CreateCbr("sample3_targetSwitch", testedMessage: "input", defaultTarget: terminator, cases: new[] {
                new CbrCase("schematron_sample3_targetA", sample3_sendToA),
                new CbrCase("schematron_sample3_targetB", sample3_sendToB),
                new CbrCase("schematron_sample3_targetC", sample3_sendToC)
            });
            var sample3_transform = MessageFlowBuilder.CreateTransformation("sample3_transform", xslt: "xslt_sample3", inputMessage: "input", outputMessage: "result", nextNode: sample3_targetSwitch);
            #endregion

            var mainSampleSwitch = MessageFlowBuilder.CreateCbr("sampleRecognition", testedMessage: "input", defaultTarget: terminator, cases: new[] {
                new CbrCase("schematron_sample1_detection", sample1_containsA),
                new CbrCase("schematron_sample2_detection", sample2_priceSumSwitch),
                new CbrCase("schematron_sample3_detection", sample3_transform)
            });

            var messageFlowConfig = new MessageFlowConfiguration("sampleMessageflow", 1) {
                Nodes = { 
                    mainSampleSwitch,
                    sample1_containsA, sample1_containsB, sample1_containsC, sample1_sendToA, sample1_sendToB, sample1_sendToC, terminator,
                    sample2_priceSumSwitch, sample2_sendToA, sample2_sendToB, sample2_sendToC,
                    sample3_transform, sample3_targetSwitch, sample3_sendToA, sample3_sendToB, sample3_sendToC
                },
                RootNode = mainSampleSwitch
            };
            #endregion

            config.AddMessageFlow(messageFlowConfig);
            config.SetCurrentMessageFlowGuid(messageFlowConfig.Guid);
            broker.ChangeConfiguration(config);
        }

        protected override void OnStop(OnStopServiceArgs args)
        {
            gateway.Stop();
            processor.Stop();
            broker.Stop();
        }
    }
}
