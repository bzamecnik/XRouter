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
using ObjectConfigurator;
using ObjectConfigurator.ValueValidators;

namespace XRouter.Gui.ConfigurationControls.Processor
{
	/// <summary>
	/// Interaction logic for ConfigurationControl.xaml
	/// </summary>
	public partial class ProcessorConfigurationControl : UserControl, IConfigurationControl
	{
		public event Action ConfigChanged = delegate { };

        public string ProcessorName { get; private set; }

        private ConfigurationTreeItem ConfigTreeItem { get; set; }
        private ConfigurationManager ConfigManager { get; set; }

        private ConfigurationEditor editor;

        public ProcessorConfigurationControl(string processorName)
		{
			InitializeComponent();
            ProcessorName = processorName;
		}

        void IConfigurationControl.Initialize(ConfigurationManager configManager, ConfigurationTreeItem configTreeItem)
        {
            ConfigManager = configManager;
            ConfigTreeItem = configTreeItem;

            Config config = new Config {
                ConcurrentProcessingThreads = ConfigManager.Configuration.GetConcurrentThreadsCountForProcessor(ProcessorName)
            };
            editor = Configurator.CreateEditor(typeof(Config));
            XDocument xConfig = Configurator.SaveConfiguration(config);
            editor.LoadConfiguration(xConfig);
            uiContent.Content = editor;
		}

        void IConfigurationControl.Save()
		{
            XDocument xConfig = editor.SaveConfiguration();
            Config config = new Config();
            Configurator.LoadConfiguration(config, xConfig);
            ConfigManager.Configuration.SetConcurrentThreadsCountForProcessor(ProcessorName, config.ConcurrentProcessingThreads);
            ConfigManager.SaveConfiguration();
		}

        private class Config
        {
            [ConfigurationItem("Concurrent processing threads", null, 4)]
            [RangeValidator(1, 100)]
            public int ConcurrentProcessingThreads { get; set; }
        }
	}
}
