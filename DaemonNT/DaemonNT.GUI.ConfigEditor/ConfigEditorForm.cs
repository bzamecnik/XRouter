using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DaemonNT;
using DaemonNT.Configuration;
using System.Xml.Linq;

namespace DaemonNT.GUI.ConfigEditor
{
    public partial class ConfigEditorForm : Form
    {
        string configFileName = string.Empty;
        XDocument xmlConfiguration = new XDocument();
        Configuration.Configuration configuration = new Configuration.Configuration();

        SettingsEditorForm settingsEditorForm = new SettingsEditorForm();

        public ConfigEditorForm()
        {
            InitializeComponent();

            installerStartTypeComboBox.Items.AddRange(
                InstallerSettings.VALID_START_MODE_VALUES.ToArray());
            installerStartTypeComboBox.Text = InstallerSettings.DEFAULT_START_MODE_VALUE;

            installerAccountComboBox.Items.AddRange(
                InstallerSettings.VALID_ACCOUNT_VALUES.ToArray());
            installerAccountComboBox.Text = InstallerSettings.DEFAULT_ACCOUNT_VALUE;
        }

        private void editServiceSettingsButton_Click(object sender, EventArgs e)
        {
            var serviceSettings = GetSelectedServiceSettings();
            Settings settings = (serviceSettings != null) ? serviceSettings.Settings : null;
            settingsEditorForm.Settings = settings;
            settingsEditorForm.ShowDialog();

            // get settingsForm.Settings
        }

        private void editStorageSettingsButton_Click(object sender, EventArgs e)
        {
            var storageSettings = GetSelectedTraceLoggerStorageSettings();
            settingsEditorForm.Settings = storageSettings;
            settingsEditorForm.ShowDialog();

            // get settingsForm.Settings
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            OpenConfigFile();
        }

        private void OpenConfigFile()
        {
            if (openConfigFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string newConfigFile = openConfigFileDialog.FileName;

            try
            {
                XDocument newXmlConfiguration = ConfigProvider.LoadRawConfiguration(newConfigFile);
                configFileName = newConfigFile;
                xmlConfiguration = newXmlConfiguration;
                configuration = ConfigProvider.ConfigurationFromXML(xmlConfiguration);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(string.Format("Cannot open file {0}. Error: {1}",
                    newConfigFile, ex.Message), "Error");
            }
            FillConfigToGUI();
        }

        private void SaveConfigFile()
        {
            throw new NotImplementedException();
        }

        private void FillConfigToGUI()
        {
            ClearForm();

            if ((configuration == null) || (configuration.Services == null))
            {
                return;
            }

            var serviceNames = configuration.Services.Select((service) => service.Name);
            servicesListBox.Items.AddRange(serviceNames.ToArray());

            if (configuration.Services.Count > 0)
            {
                // NOTE: this fills the service settings panel
                servicesListBox.SelectedIndex = 0;
            }

            xmlSourceTextBox.Text = xmlConfiguration.ToString();

            //throw new NotImplementedException();
        }

        private void FillServiceToGUI(ServiceSettings service)
        {
            serviceNameTextBox.Text = service.Name;
            serviceTypeClassTextBox.Text = service.TypeClass;
            serviceTypeAssemblyTextBox.Text = service.TypeAssembly;

            InstallerSettings installer = service.InstallerSettings;
            installerDescriptionTextBox.Text = installer.Description;
            installerStartTypeComboBox.Text = installer.StartMode;
            installerAccountComboBox.Text = installer.Account;
            installerUsernameTextBox.Text = installer.User;
            installerPasswordTextBox.Text = installer.Password;
            installerRequiredServicesListBox.Items.Clear();
            installerRequiredServicesListBox.Items.AddRange(
                installer.RequiredServices.ToArray());

            TraceLoggerSettings traceLogger = service.TraceLoggerSettings;
            traceLoggerBufferSizeNumeric.Value = traceLogger.BufferSize;
            traceLoggerStoragesListBox.Items.Clear();
            var storageNames = traceLogger.Storages.Select((storage) => storage.Name);
            traceLoggerStoragesListBox.Items.AddRange(storageNames.ToArray());
            if (traceLogger.Storages.Count > 0)
            {
                // NOTE: this fills the service settings panel
                traceLoggerStoragesListBox.SelectedIndex = 0;
            }
        }

        private void FillConfigFromGUI()
        {
            throw new NotImplementedException();
        }

        private void ClearForm()
        {
            servicesListBox.Items.Clear();
            installerRequiredServicesListBox.Items.Clear();
            traceLoggerStoragesListBox.Items.Clear();

            xmlSourceTextBox.Text = string.Empty;

            //throw new NotImplementedException();
        }

        private void servicesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ServiceSettings serviceSettings = GetSelectedServiceSettings();
            if (serviceSettings == null)
            {
                // TODO: clear the controls
                return;
            }

            FillServiceToGUI(serviceSettings);
        }

        private ServiceSettings GetSelectedServiceSettings()
        {
            string selectedServiceName = (string)servicesListBox.SelectedItem;
            if (selectedServiceName == null)
            {
                return null;
            }
            ServiceSettings serviceSettings = configuration.Services.FirstOrDefault(
                (service) => service.Name == selectedServiceName);
            return serviceSettings;
        }

        private Settings GetSelectedTraceLoggerStorageSettings()
        {
            var serviceSettings = GetSelectedServiceSettings();
            if ((serviceSettings == null) ||
                (serviceSettings.TraceLoggerSettings == null) ||
                (serviceSettings.TraceLoggerSettings.Storages == null))
            {
                return null;
            }
            string selectedStorageName = (string)traceLoggerStoragesListBox.SelectedItem;
            if (selectedStorageName == null)
            {
                return null;
            }
            var storageSettings = serviceSettings.TraceLoggerSettings.Storages.FirstOrDefault(
                (storage) => storage.Name == selectedStorageName);
            if (storageSettings == null)
            {
                return null;
            }
            return storageSettings.Settings;
        }

        private void traceLoggerStoragesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedStorageName = (string)traceLoggerStoragesListBox.SelectedItem;

            if (selectedStorageName == null)
            {
                // TODO: clear the controls
                return;
            }

            ServiceSettings serviceSettings = GetSelectedServiceSettings();
            if (serviceSettings == null)
            {
                // TODO: clear the controls
                return;
            }

            TraceLoggerStorageSettings storageSettings =
                serviceSettings.TraceLoggerSettings.Storages.FirstOrDefault(
                (storage) => storage.Name == selectedStorageName);

            if (storageSettings == null)
            {
                // TODO: clear the controls
                return;
            }

            traceLoggerStorageNameTextBox.Text = storageSettings.Name;
            traceLoggerStorageClassTextBox.Text = storageSettings.TypeClass;
            traceLoggerStorageAssemblyTextBox.Text = storageSettings.TypeAssembly;
        }
    }
}
