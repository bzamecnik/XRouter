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
using XRouter.Common;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common.MessageFlowConfig;

namespace XRouter.Gui.ConfigurationControls.Messageflow
{
    /// <summary>
    /// Interaction logic for MessageflowConfigurationControl.xaml
    /// </summary>
    public partial class MessageflowConfigurationControl : UserControl, IConfigurationControl
    {
        public bool IsDirty
        {
            get { throw new NotImplementedException(); }
        }

        private IBrokerServiceForManagement brokerService;
        private ConfigurationTree configTreeNode;
        private ApplicationConfiguration appConfig;

        public MessageflowConfigurationControl()
        {
            InitializeComponent();
        }

        public void Initialize(ApplicationConfiguration appConfig, IBrokerServiceForManagement brokerService, ConfigurationTree configTreeNode)
        {
            this.appConfig = appConfig;
            this.brokerService = brokerService;
            this.configTreeNode = configTreeNode;


            MessageFlowConfiguration messageflow = appConfig.GetMessageFlow(appConfig.GetCurrentMessageFlowGuid());
            MessageflowGraphPresenter graphPresenter = new MessageflowGraphPresenter(messageflow);
            uiDesignerContainer.Child = graphPresenter.CreateGraphCanvas();
        }

        public void Save()
        {
        }

        public void Clear()
        {
        }
    }
}
