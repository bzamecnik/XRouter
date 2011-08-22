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
        public event Action ConfigChanged = delegate { };

        private ConfigurationTreeItem ConfigTreeItem { get; set; }
        private ConfigurationManager ConfigManager { get; set; }

        private AdapterConfiguration adapter;
        private AdapterType adapterType;
        private ConfigurationEditor editor;

        public AdapterConfigurationControl(AdapterConfiguration adapter)
		{
			InitializeComponent();

            this.adapter = adapter;
		}

        void IConfigurationControl.Initialize(ConfigurationManager configManager, ConfigurationTreeItem configTreeItem)
        {
            ConfigManager = configManager;
            ConfigTreeItem = configTreeItem;

            adapterType = configManager.Configuration.GetAdapterType(adapter.AdapterTypeName);

            editor = Configurator.CreateEditor(adapterType.ConfigurationMetadata);
            editor.LoadConfiguration(adapter.Configuration);
            uiEditorContainer.Child = editor;
		}

        void IConfigurationControl.Save()
		{
            adapter.Configuration = new SerializableXDocument(editor.SaveConfiguration());
            ConfigManager.Configuration.SaveAdapterConfiguration(adapter);
		}
	}
}
