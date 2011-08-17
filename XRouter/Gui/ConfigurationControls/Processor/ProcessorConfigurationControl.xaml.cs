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

namespace XRouter.Gui.ConfigurationControls.Processor
{
	/// <summary>
	/// Interaction logic for ConfigurationControl.xaml
	/// </summary>
	public partial class ProcessorConfigurationControl : UserControl, IConfigurationControl
	{
		private bool isdirty = false;
        private XElement oldConfiguration;

        private IBrokerServiceForManagement brokerService;
        private ConfigurationTree configTreeNode;
        private ApplicationConfiguration appConfig;

        //public XElement Configuration;

        public ProcessorConfigurationControl()
		{
			InitializeComponent();
		}

		public bool IsDirty
		{
			get { return isdirty; }
		}

        public void Initialize(ApplicationConfiguration appConfig, IBrokerServiceForManagement brokerService, ConfigurationTree configTreeNode)
		{
            this.appConfig = appConfig;
            this.brokerService = brokerService;
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
