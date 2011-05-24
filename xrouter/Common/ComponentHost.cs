using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaemonNT;
using System.IO;
using System.Reflection;

namespace XRouter.Common
{
    public class ComponentHost : Service
    {
        private static readonly string SettingsKey_ComponentType = "ComponentType";
        private static readonly string SettingsKey_ComponentName = "ComponentName";
        private static readonly string SettingsKey_ComponentSettingsSection = "ComponentSettings";

        private IHostableComponent Component { get; set; }

        protected override void OnStart(OnStartServiceArgs args)
        {
            TraceLog.Initialize(Logger);
            EventLog.Initialize(Logger);

            #region Prepare Component
            string componentClassAndAssembly = args.Settings.Parameters[SettingsKey_ComponentType];
            object componentObject = CreateTypeInstance(componentClassAndAssembly);
            Component = componentObject as IHostableComponent;
            if (Component == null) {
                throw new InvalidOperationException(string.Format("Type '{0}' does not implement '{1}'.", 
                    componentClassAndAssembly, typeof(IHostableComponent).FullName));
            }
            #endregion

            #region Prepare componentSettings
            var componentSettingsSection = args.Settings[SettingsKey_ComponentSettingsSection];
            if (componentSettingsSection == null) {
                throw new InvalidOperationException( string.Format("Missing settings section '{0}'", SettingsKey_ComponentSettingsSection));
            }
            var componentSettings = new Dictionary<string, string>();
            foreach (string key in componentSettingsSection.Parameters.Keys) {
                string value = componentSettingsSection.Parameters[key];
                componentSettings.Add(key, value);
            }
            #endregion

            string componentName = args.Settings.Parameters[SettingsKey_ComponentName];
            if (componentName == null) {
                throw new InvalidOperationException(string.Format("Missing component parameter '{0}'.", SettingsKey_ComponentName));
            }

            Component.Start(componentName, componentSettings);
        }

        protected override void OnStop(OnStopServiceArgs args)
        {
            Component.Stop();
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
