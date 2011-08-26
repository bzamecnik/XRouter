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

namespace XRouter.Gui.ConfigurationControls.Gateway
{
	/// <summary>
	/// Interaction logic for ConfigurationControl.xaml
	/// </summary>
	public partial class GatewayConfigurationControl : UserControl, IConfigurationControl
	{
        public event Action ConfigChanged = delegate { };

        private ConfigurationTreeItem ConfigTreeItem { get; set; }
        private ConfigurationManager ConfigManager { get; set; }

        public string GatewayName { get; private set; }

        private AdapterConfiguration activeAdapter;
        private ConfigurationEditor activeConfigurationEditor;

        public GatewayConfigurationControl(string gatewayName)
		{
			InitializeComponent();
            GatewayName = gatewayName;
		}

        void IConfigurationControl.Initialize(ConfigurationManager configManager, ConfigurationTreeItem configTreeItem)
        {
            ConfigManager = configManager;
            ConfigTreeItem = configTreeItem;

            #region Prepare adapters editing
            uiAdaptersListEditor.Initialize(AddAdapter);
            uiAdaptersListEditor.ItemRemoved += RemoveAdapter;
            uiAdaptersListEditor.ItemSelected += SetActiveAdapter;

            uiAdapterConfigurationRegion.Visibility = Visibility.Collapsed;
            AdapterConfiguration[] adapters = configManager.Configuration.GetAdapterConfigurations(GatewayName);
            foreach (AdapterConfiguration adapeter in adapters) {
                uiAdaptersListEditor.AddItem(CreateAdapterRepresentation(adapeter));
            }
            uiAdaptersListEditor.SelectItem(uiAdaptersListEditor.Items.FirstOrDefault());
            #endregion
		}

        #region Adapters editing

        private FrameworkElement AddAdapter()
        {
            #region Create unique name
            var adapterNames = ConfigManager.Configuration.GetAdapterConfigurations(GatewayName).Select(a => a.AdapterName);
            int index = 1;
            while (adapterNames.Contains("adapter" + index.ToString())) {
                index++;
            }
            string adapterName = "adapter" + index.ToString();
            #endregion

            AdapterType adapterType = ConfigManager.Configuration.GetAdapterTypes().First();
            AdapterConfiguration adapter = new AdapterConfiguration(adapterName, GatewayName, adapterType.Name);
            adapter.Configuration = new SerializableXDocument(new XDocument());
            adapter.Configuration.XDocument.Add(new XElement(XName.Get("objectConfig")));
            ConfigManager.Configuration.SaveAdapterConfiguration(adapter);

            FrameworkElement adapterRepresentation = CreateAdapterRepresentation(adapter);
            return adapterRepresentation;
        }

        private FrameworkElement CreateAdapterRepresentation(AdapterConfiguration adapter)
        {
            TextBox uiName = new TextBox {
                Text = adapter.AdapterName,
                Width = 150,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 0, 0, 0)
            };
            uiName.LostFocus += delegate {
                string originalName = adapter.AdapterName;
                var adapterNames = ConfigManager.Configuration.GetAdapterConfigurations(GatewayName).Where(a => a.AdapterName != originalName).Select(a => a.AdapterName);
                string newName = uiName.Text;
                while (adapterNames.Contains(newName)) {
                    newName = newName + " (new)";
                }

                uiName.Text = newName;
                adapter.AdapterName = newName;
                ConfigManager.Configuration.SaveAdapterConfiguration(adapter, originalName);
            };
            Grid.SetColumn(uiName, 1);

            string[] adapterTypeNames = ConfigManager.Configuration.GetAdapterTypes().Select(at => at.Name).ToArray();
            ComboBox uiAdapterType = new ComboBox {
                Tag = adapter,
                Margin = new Thickness(10, 3, 5, 3),
                IsEditable = false,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            foreach (string adapterTypeName in adapterTypeNames) {
                uiAdapterType.Items.Add(new ComboBoxItem {
                    Tag = adapterTypeName,
                    Content = adapterTypeName
                });
            }
            uiAdapterType.SelectedIndex = adapterTypeNames.ToList().IndexOf(adapter.AdapterTypeName);
            uiAdapterType.SelectionChanged += delegate {
                string selectedAdapterTypeName = (string)(((ComboBoxItem)uiAdapterType.SelectedItem).Tag);
                adapter.AdapterTypeName = selectedAdapterTypeName;
                ConfigManager.Configuration.SaveAdapterConfiguration(adapter);
                SetActiveAdapter(uiAdapterType);
            };
            Grid.SetColumn(uiAdapterType, 2);

            var uiAdapter = new Grid {
                Tag = adapter,
                ColumnDefinitions = {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                Children = {
                    new Image {
                        Source = new BitmapImage(new Uri("pack://application:,,,/XRouter.Gui;component/Resources/Generic_Device.png")),
                        Height = 18,
                        Margin = new Thickness(8, 0, 0, 0)
                    },
                    uiName,
                    uiAdapterType
                }
            };

            return uiAdapter;
        }

        private void RemoveAdapter(FrameworkElement uiAdapter)
        {
            AdapterConfiguration adapter = (AdapterConfiguration)uiAdapter.Tag;
            ConfigManager.Configuration.RemoveAdapterConfiguration(adapter);
        }

        private void SetActiveAdapter(FrameworkElement uiAdapter)
        {
            if (uiAdapter == null) {
                activeAdapter = null;
                uiAdapterConfigurationRegion.Visibility = Visibility.Collapsed;
                return;
            }

            activeAdapter = (AdapterConfiguration)uiAdapter.Tag;
            uiAdapterConfigurationRegion.Visibility = Visibility.Visible;
            uiAdapterConfigurationHeader.Text = activeAdapter.AdapterName;

            AdapterType activeAdapterType = ConfigManager.Configuration.GetAdapterType(activeAdapter.AdapterTypeName);
            activeConfigurationEditor = Configurator.CreateEditor(activeAdapterType.ConfigurationMetadata);
            activeConfigurationEditor.LoadConfiguration(activeAdapter.Configuration.XDocument);
            activeConfigurationEditor.ConfigurationChanged += delegate {
                activeAdapter.Configuration = new SerializableXDocument(activeConfigurationEditor.SaveConfiguration());
                ConfigManager.Configuration.SaveAdapterConfiguration(activeAdapter);
            };
            uiAdapterConfigurationContainer.Child = activeConfigurationEditor;
        }

        #endregion

        void IConfigurationControl.Save()
		{
		}
	}
}
