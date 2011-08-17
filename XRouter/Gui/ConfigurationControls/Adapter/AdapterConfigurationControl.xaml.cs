using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using XRouter.Common.Utils;
using ObjectConfigurator;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common;
using XRouter.Manager;

namespace XRouter.Gui.ConfigurationControls.Adapter
{
	/// <summary>
	/// Interaction logic for ConfigurationControl.xaml
	/// </summary>
	public partial class AdapterConfigurationControl : UserControl, IConfigurationControl
	{
		public bool IsDirty
		{
			get { throw new NotImplementedException(); }
		}

        private ConfigurationEditor editor;
        private XElement originalConfig;
        private XDocument originalObjectConfig;
        private string adapterName;

        private IConsoleServer consoleServer;
        private ConfigurationTree configTreeNode;
        private ApplicationConfiguration appConfig;

        public AdapterConfigurationControl()
        {
            InitializeComponent();
        }

        public void Initialize(ApplicationConfiguration appConfig, IConsoleServer consoleServer, ConfigurationTree configTreeNode)
		{
            this.appConfig = appConfig;
            this.consoleServer = consoleServer;
            this.configTreeNode = configTreeNode;
            this.originalConfig = configTreeNode.XContent;

            adapterName = originalConfig.Attribute(XName.Get("name")).Value;
            string typeAndAssemblyFullName = originalConfig.Attribute(XName.Get("type")).Value;
            Type type = TypeUtils.GetType(typeAndAssemblyFullName);

            XElement xConfig = originalConfig.Element(XName.Get("objectConfig"));
            originalObjectConfig = new XDocument();
            originalObjectConfig.Add(xConfig);

            editor = Configurator.CreateEditor(type);
            editor.LoadConfiguration(originalObjectConfig);
            uiEditorContainer.Child = editor;
		}

        public void Save()
		{
            var xConfig = editor.SaveConfiguration().Root;
            string gatewayName = configTreeNode.Parent.Name;

            var xObjectConfig = originalConfig.Element(XName.Get("objectConfig"));
            xObjectConfig.Remove();
            originalConfig.Add(editor.SaveConfiguration().Root);

            //appConfig.SaveAdapterConfiguration(gatewayName, originalConfig);
		}

		public void Clear()
		{
            editor.LoadConfiguration(originalObjectConfig);
		}
	}
}
