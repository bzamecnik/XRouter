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
using XRouter.Common.ComponentInterfaces;
using XRouter.Common;
using XRouter.Manager;

namespace XRouter.Gui.ConfigurationControls.Dispatcher
{
	/// <summary>
	/// Interaction logic for ConfigurationControl.xaml
	/// </summary>
	public partial class DispatcherConfigurationControl : UserControl, IConfigurationControl
	{
		private bool isdirty = false;
        private XElement oldConfiguration;

        private IConsoleServer consoleServer;
        private ConfigurationTree configTreeNode;
        private ApplicationConfiguration appConfig;

        //public XElement Configuration;

        public DispatcherConfigurationControl()
		{
			InitializeComponent();
		}

		public bool IsDirty
		{
			get { return isdirty; }
		}

        public void Initialize(ApplicationConfiguration appConfig, IConsoleServer consoleServer, ConfigurationTree configTreeNode)
		{
            this.appConfig = appConfig;
            this.consoleServer = consoleServer;
            this.configTreeNode = configTreeNode;
            this.oldConfiguration = configTreeNode.XContent;
            this.DataContext = configTreeNode.XContent;
		}

        public void Save()
		{
			isdirty = false;
			//return this.DataContext as XElement;
		}

		public void Clear()
		{
			this.DataContext = this.oldConfiguration;
		}
	}
}
