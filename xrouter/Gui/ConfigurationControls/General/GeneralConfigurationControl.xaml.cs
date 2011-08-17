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

namespace XRouter.Gui.ConfigurationControls.General
{
	/// <summary>
	/// Interaction logic for ConfigurationControl.xaml
	/// </summary>
	public partial class GeneralConfigurationControl : UserControl, IConfigurationControl
	{
        public event Action ConfigChanged = delegate { };

        private ConfigurationTreeItem ConfigTreeItem { get; set; }
        private ConfigurationManager ConfigManager { get; set; }

        public GeneralConfigurationControl()
		{
			InitializeComponent();
		}

        void IConfigurationControl.Initialize(ConfigurationManager configManager, ConfigurationTreeItem configTreeItem)
        {
            ConfigManager = configManager;
            ConfigTreeItem = configTreeItem;
		}

        void IConfigurationControl.Save()
		{
		}
	}
}
