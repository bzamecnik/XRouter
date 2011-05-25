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
        }

        protected override void OnStop(OnStopServiceArgs args)
        {
            gateway.Stop();
            processor.Stop();
            broker.Stop();
        }
    }
}
