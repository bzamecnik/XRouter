/*! \mainpage XRouter - API reference
 *
 * %XRouter can integrate various systems communicating via XML messages. It is
 * a simple, easy-to-use, configurable, light-weight, efficient alternative to
 * complex enterprise service bus (ESB) solutions. It is well-designed, thoroughly
 * documented and released as an open-source software.

 * The %XRouter project consists of several subprojects:
 * \li %XRouter service - light-weight extensible low-level XML router for
 * .NET, the main project, plus a configuration management GUI and a simple
 * monitoring service
 * \li %SchemaTron - native C# validator of ISO Schematron language
 * \li %DaemonNT - Windows service hosting made easy
 * \li %ObjectConfigurator - reflection-based configuration utility
 * \li %SimpleDiagrammer - interactive visualizer of oriented graphs
 * 
 * Please find more information in the full documentation which can be found
 * at the project home page: http://assembla.com/spaces/xrouter .
 */

using DaemonNT;
using XRouter.Common;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common.MessageFlowConfig;
using XRouter.Common.Xrm;
using XRouter.Processor.MessageFlowBuilding;

namespace XRouter.ComponentHosting
{
    /// <summary>
    /// A DaemonNT service which runs all the XRouter service components
    /// within a single process (in multiple threads).
    /// </summary>
    class XRouterService : Service
    {
        private static readonly string SectionKey_Broker = "broker";
        private static readonly string SectionKey_Gateway = "gateway";
        private static readonly string SectionKey_Processor = "processor";
        private static readonly string SettingsKey_ComponentName = "component-name";
        private static readonly string SettingsKey_ConnectionString = "connection-string";

        private IBrokerService broker;
        private IGatewayService gateway;
        private IProcessorService processor;

        protected override void OnStart(OnStartServiceArgs args)
        {
            TraceLog.Initialize(Logger);
            EventLog.Initialize(Logger);

            ObjectConfigurator.Configurator.CustomItemTypes.Add(new TokenSelectionConfigurationItemType());
            ObjectConfigurator.Configurator.CustomItemTypes.Add(new XrmUriConfigurationItemType());
            ObjectConfigurator.Configurator.CustomItemTypes.Add(new UriConfigurationItemType());

            broker = new XRouter.Broker.BrokerService();
            processor = new XRouter.Processor.ProcessorService();
            gateway = new XRouter.Gateway.Gateway();

            string processorName = args.Settings[SectionKey_Processor].Parameters[SettingsKey_ComponentName];
            string gatewayName = args.Settings[SectionKey_Gateway].Parameters[SettingsKey_ComponentName];
            string connectionString = args.Settings[SectionKey_Broker].Parameters[SettingsKey_ConnectionString];

            broker.Start(
                connectionString,
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
