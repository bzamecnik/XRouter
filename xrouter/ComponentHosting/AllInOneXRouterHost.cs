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

            gateway = new XRouter.Gateway.Implementation.Gateway();
            string gatewayName = args.Settings[SectionKey_Gateway].Parameters[SettingsKey_ComponentName];
            gateway.Start(gatewayName, broker);
        }

        protected override void OnStop(OnStopServiceArgs args)
        {
            gateway.Stop();
            processor.Stop();
            broker.Stop();
        }

        private static object CreateTypeInstance(string typeAndAssembly)
        {
            string typeFullName;
            string assemblyPath;
            string[] parts = typeAndAssembly.Split(',');
            if (parts.Length == 2) {
                typeFullName = parts[0].Trim();
                assemblyPath = parts[1].Trim();
            } else if (parts.Length == 1) {
                typeFullName = parts[0].Trim();
                assemblyPath = null;
            } else {
                throw new InvalidOperationException(string.Format("Invalid type identification: '{0}'", typeAndAssembly));
            }

            return CreateTypeInstance(typeFullName, assemblyPath);
        }

        private static object CreateTypeInstance(string typeFullName, string assemblyPath)
        {
            if ((assemblyPath != null) && (!Path.IsPathRooted(assemblyPath))) {
                assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyPath);
            }

            #region Prepare type
            Type type;
            try {
                if (assemblyPath != null) {
                    Assembly assembly = Assembly.LoadFrom(assemblyPath);
                    type = assembly.GetType(typeFullName, true);
                } else {
                    type = Type.GetType(typeFullName, true);
                }
            } catch (Exception ex) {
                throw new InvalidOperationException(string.Format("Cannot access type '{0}'.", typeFullName), ex);
            }
            #endregion

            #region Create instance
            object instance;
            try {
                instance = Activator.CreateInstance(type, true);
            } catch (Exception ex) {
                throw new InvalidOperationException(string.Format("Cannot create instance of type '{0}' using default constructor.", typeFullName), ex);
            }
            #endregion

            return instance;
        }

    }
}
