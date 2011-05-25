using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaemonNT;
using XRouter.Common;
using XRouter.Broker;
using System.Reflection;
using System.IO;

namespace XRouter.ComponentHosting
{
    class AllInOneXRouterHost : Service
    {
        private static readonly string SectionKey_Broker = "Broker";
        private static readonly string SectionKey_Gateway = "Gateway";
        private static readonly string SectionKey_Processor = "Processor";
        private static readonly string SettingsKey_ComponentName = "ComponentName";

        private IBrokerService broker;
        private IGatewayService gateway;
        private IProcessorService processor;

        protected override void OnStart(OnStartServiceArgs args)
        {
            TraceLog.Initialize(Logger);
            EventLog.Initialize(Logger);

            broker = new XRouter.Broker.BrokerService();
            broker.Start();

            processor = new XRouter.Processor.ProcessorService();
            string processorName = args.Settings[SectionKey_Processor].Parameters[SettingsKey_ComponentName];
            processor.Start(processorName, broker);

            gateway = new XRouter.Gateway.Gateway();
            string gatewayName = args.Settings[SectionKey_Gateway].Parameters[SettingsKey_ComponentName];
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
