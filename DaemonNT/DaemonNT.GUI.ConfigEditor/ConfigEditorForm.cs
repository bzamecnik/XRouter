﻿using System;
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
        string configFileName;
        string ConfigFileName
        {
            get { return configFileName; }
            set
            {
                configFileName = value;
                configFileToolStripStatusLabel.Text =
                    !string.IsNullOrEmpty(configFileName)
                    ? System.IO.Path.GetFileName(configFileName)
                    : "<unsaved file>";
            }
        }
        XDocument xmlConfiguration = new XDocument();
        Configuration.Configuration configuration = new Configuration.Configuration();
        string currentServiceName = string.Empty;
        ServiceSettings currentService = new ServiceSettings();
        string currentTraceLoggerStorageName = string.Empty;
        TraceLoggerStorageSettings currentTraceLoggerStorage = null;

        List<string> serviceNames = new List<string>();
        List<string> installerRequiredServiceNames = new List<string>();
        List<string> traceLoggerStorageNames = new List<string>();

        SettingsEditorForm settingsEditorForm = new SettingsEditorForm();
        UniqueNameForm serviceNameForm = new UniqueNameForm("Service name", "Enter service name (must be unique):");
        UniqueNameForm storageNameForm = new UniqueNameForm("Storage name", "Enter storage name (must be unique):");

        public ConfigEditorForm()
        {
            InitializeComponent();

            ConfigFileName = string.Empty;

            servicesListBox.DataSource = serviceNames;
            installerRequiredServicesListBox.DataSource = installerRequiredServiceNames;
            traceLoggerStoragesListBox.DataSource = traceLoggerStorageNames;

            installerStartTypeComboBox.Items.AddRange(
                InstallerSettings.ValidStartModeValues.ToArray());
            installerStartTypeComboBox.Text = InstallerSettings.DefaultStartModeValue;

            installerAccountComboBox.Items.AddRange(
                InstallerSettings.ValidAccountValues.ToArray());
            installerAccountComboBox.Text = InstallerSettings.DefaultAccountValue;

            traceLoggerBufferSizeNumeric.Value = TraceLoggerSettings.DefaultBufferSize;
        }

        private void editServiceSettingsButton_Click(object sender, EventArgs e)
        {
            settingsEditorForm.Settings = DeepClone(currentService.Settings);
            if (settingsEditorForm.ShowDialog() == DialogResult.OK)
            {
                currentService.Settings = DeepClone(settingsEditorForm.Settings);
            }
        }

        private void editStorageSettingsButton_Click(object sender, EventArgs e)
        {
            settingsEditorForm.Settings = DeepClone(currentTraceLoggerStorage.Settings);
            if (settingsEditorForm.ShowDialog() == DialogResult.OK)
            {
                currentTraceLoggerStorage.Settings = DeepClone(settingsEditorForm.Settings);
            }
        }

        private void openConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenConfigFileWithDialog();
        }

        private void OpenConfigFileWithDialog()
        {
            if (openConfigFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string newConfigFile = openConfigFileDialog.FileName;

            try
            {
                XDocument newXmlConfiguration = ConfigProvider.LoadRawConfiguration(newConfigFile);
                ConfigFileName = newConfigFile;
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

        private void SaveConfigFileWithDialog()
        {
            if (saveConfigFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            ConfigFileName = saveConfigFileDialog.FileName;
            SaveConfigFile(ConfigFileName);
        }

        private void SaveConfigFile(string fileName)
        {
            try
            {
                FillConfigFromGUI();
                xmlConfiguration = ConfigProvider.ConfigurationToXML(configuration);
                xmlConfiguration.Save(ConfigFileName);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(string.Format("Cannot save file {0}. Error: {1}",
                    ConfigFileName, ex.Message), "Error");
            }
        }

        private void FillConfigToGUI()
        {
            ClearForm();

            if ((configuration == null) || (configuration.Services == null))
            {
                return;
            }

            serviceNames.Clear();
            serviceNames.AddRange(configuration.Services.Select((service) => service.Name));
            RefreshListBox(servicesListBox);

            currentServiceName = null;
            currentService = new ServiceSettings();
            servicesListBox.SelectedIndex = -1;
            if (configuration.Services.Count > 0)
            {
                // NOTE: this fills the service settings panel
                servicesListBox.SelectedIndex = 0;
            }

            xmlSourceTextBox.Text = xmlConfiguration.ToString();
        }

        private void FillServiceToGUI(ServiceSettings service)
        {
            serviceNameTextBox.Text = service.Name;
            serviceTypeClassTextBox.Text = service.TypeClass;
            serviceTypeAssemblyTextBox.Text = service.TypeAssembly;

            InstallerSettings installer = service.InstallerSettings;
            if (installer != null)
            {
                installerDescriptionTextBox.Text = installer.Description;
                installerStartTypeComboBox.Text = installer.StartMode;
                installerAccountComboBox.Text = installer.Account;
                installerUsernameTextBox.Text = installer.User;
                installerPasswordTextBox.Text = installer.Password;
                installerRequiredServiceNames.Clear();
                installerRequiredServiceNames.AddRange(installer.RequiredServices);
                RefreshListBox(installerRequiredServicesListBox);
            }

            TraceLoggerSettings traceLogger = service.TraceLoggerSettings;
            currentTraceLoggerStorageName = null;
            currentTraceLoggerStorage = null;
            if (traceLogger != null)
            {
                traceLoggerBufferSizeNumeric.Value = traceLogger.BufferSize;
                traceLoggerStorageNames.Clear();
                traceLoggerStorageNames.AddRange(traceLogger.Storages.Select((storage) => storage.Name));
                RefreshListBox(traceLoggerStoragesListBox);
                traceLoggerStoragesListBox.SelectedIndex = -1;
                traceLoggerStorageGroupBox.Enabled = false;
                if (traceLogger.Storages.Count > 0)
                {
                    // NOTE: this fills the service settings panel
                    traceLoggerStoragesListBox.SelectedIndex = 0;
                }
                currentTraceLoggerStorageName = (string)traceLoggerStoragesListBox.SelectedItem;
            }
        }

        private void FillConfigFromGUI()
        {
            FillServiceFromGUI();
        }

        private void FillServiceFromGUI()
        {
            string serviceName = GetSelectedServiceName();
            if (serviceName == null)
            {
                return;
            }

            if (currentService == null)
            {
                currentService = new ServiceSettings();
            }

            currentService.Name = serviceNameTextBox.Text;
            currentService.TypeClass = serviceTypeClassTextBox.Text;
            currentService.TypeAssembly = serviceTypeAssemblyTextBox.Text;

            currentService.InstallerSettings = new InstallerSettings()
            {
                Description = installerDescriptionTextBox.Text,
                StartMode = installerStartTypeComboBox.Text,
                Account = installerAccountComboBox.Text,
                User = installerUsernameTextBox.Text,
                Password = installerPasswordTextBox.Text,
                RequiredServices = installerRequiredServiceNames
            };

            if (currentService.TraceLoggerSettings == null)
            {
                currentService.TraceLoggerSettings = new TraceLoggerSettings();
            }
            currentService.TraceLoggerSettings.BufferSize = (int)traceLoggerBufferSizeNumeric.Value;
            // TODO: trace logger storages
            //currentService.TraceLoggerSettings.Storages.Add(currentTraceLoggerStorage);
        }

        private void FillTraceLoggerStorageToGUI(TraceLoggerStorageSettings storage)
        {
            traceLoggerStorageGroupBox.Enabled = storage != null;
            if (storage == null)
            {
                storage = new TraceLoggerStorageSettings();
            }
            traceLoggerStorageNameTextBox.Text = storage.Name;
            traceLoggerStorageClassTextBox.Text = storage.TypeClass;
            traceLoggerStorageAssemblyTextBox.Text = storage.TypeAssembly;
        }

        private void FillTraceLoggerStorageFromGUI()
        {
            if (currentTraceLoggerStorage != null)
            {
                currentTraceLoggerStorage.Name = traceLoggerStorageNameTextBox.Text;
                currentTraceLoggerStorage.TypeClass = traceLoggerStorageClassTextBox.Text;
                currentTraceLoggerStorage.TypeAssembly = traceLoggerStorageAssemblyTextBox.Text;
            }
        }


        private void ClearForm()
        {
            serviceNameTextBox.Text = string.Empty;
            serviceTypeClassTextBox.Text = string.Empty;
            serviceTypeAssemblyTextBox.Text = string.Empty;

            installerDescriptionTextBox.Text = string.Empty;
            installerStartTypeComboBox.Text = InstallerSettings.DefaultStartModeValue;
            installerAccountComboBox.Text = InstallerSettings.DefaultAccountValue;
            installerUsernameTextBox.Text = string.Empty;
            installerPasswordTextBox.Text = string.Empty;

            currentTraceLoggerStorage = null;
            FillTraceLoggerStorageToGUI(currentTraceLoggerStorage);

            traceLoggerBufferSizeNumeric.Value = TraceLoggerSettings.DefaultBufferSize;

            //serviceNames.Clear();
            //RefreshListBox(servicesListBox);
            installerRequiredServiceNames.Clear();
            RefreshListBox(installerRequiredServicesListBox);
            traceLoggerStorageNames.Clear();
            RefreshListBox(traceLoggerStoragesListBox);
        }

        private void servicesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentServiceName = GetSelectedServiceName();
            currentService = null;
            ClearForm();
            currentService = GetSelectedServiceSettings();
            if (currentService != null)
            {
                FillServiceToGUI(currentService);
                serviceGroupBox.Enabled = true;
            }
        }

        private string GetSelectedServiceName()
        {
            return (string)servicesListBox.SelectedItem;
        }

        private ServiceSettings GetSelectedServiceSettings()
        {
            string selectedServiceName = GetSelectedServiceName();
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
            RefreshTraceLogStorage();
            FillTraceLoggerStorageToGUI(currentTraceLoggerStorage);
        }

        private void RefreshTraceLogStorage()
        {
            string selectedStorageName = (string)traceLoggerStoragesListBox.SelectedItem;

            if (selectedStorageName == null)
            {
                currentTraceLoggerStorage = null;
                FillTraceLoggerStorageToGUI(null);
                return;
            }

            currentTraceLoggerStorage =
                currentService.TraceLoggerSettings.Storages.FirstOrDefault(
                (storage) => storage.Name == selectedStorageName);
        }

        private void saveConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ConfigFileName))
            {
                SaveConfigFile(ConfigFileName);
            }
            else
            {
                SaveConfigFileWithDialog();
            }
        }


        private void saveAsConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveConfigFileWithDialog();
        }

        private void newConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearForm();
            ConfigFileName = string.Empty;
            xmlConfiguration = new XDocument();
            xmlSourceTextBox.Text = string.Empty;
            configuration = new Configuration.Configuration();
            currentService = new ServiceSettings();
            currentTraceLoggerStorage = null;
            currentServiceName = string.Empty;
            currentTraceLoggerStorageName = string.Empty;
            serviceNames.Clear();
            RefreshListBox(servicesListBox);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: if there are unsaved changes, show confirm dialog
            Close();
        }

        private void installerRequiredServicesListBox_DoubleClick(object sender, EventArgs e)
        {

        }

        private static T DeepClone<T>(T obj) where T : class
        {
            if (obj == null)
            {
                return null;
            }
            using (var ms = new System.IO.MemoryStream())
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        private void newServiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string serviceName;
            if (!EditNameInForm(serviceNameForm, string.Empty, serviceNames, out serviceName))
            {
                return;
            }
            configuration.Services.Add(new ServiceSettings() { Name = serviceName });
            serviceNames.Add(serviceName);
            RefreshListBox(servicesListBox);
            // fill the service panel
            servicesListBox.SelectedIndex = -1;
            servicesListBox.SelectedIndex = serviceNames.IndexOf(serviceName);
        }

        private bool EditNameInForm(UniqueNameForm form, string defaultName,
            IList<string> existingNames, out string name)
        {
            form.EditedName = defaultName;
            form.ExistingNames = existingNames;
            if (form.ShowDialog() == DialogResult.OK)
            {
                name = form.EditedName;
                return true;
            }
            else
            {
                name = null;
                return false;
            }
        }

        private void removeServiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int selectedServiceIndex = servicesListBox.SelectedIndex;
            if (selectedServiceIndex < 0)
            {
                return;
            }
            serviceNames.RemoveAt(selectedServiceIndex);
            RefreshListBox(servicesListBox);
            ClearForm();
            configuration.Services.RemoveAt(selectedServiceIndex);
            currentService = new ServiceSettings();
            currentServiceName = string.Empty;

            servicesListBox.SelectedIndex = -1;
            if (serviceNames.Count() > 0)
            {
                servicesListBox.SelectedIndex = 0;
            }
            serviceGroupBox.Enabled = false;
        }

        private void RefreshListBox(ListBox listBox)
        {
            ((CurrencyManager)listBox.BindingContext[listBox.DataSource]).Refresh();
        }

        private void validateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateXml();
            SchemaTron.ValidatorResults results;
            ConfigProvider.IsValid(ConfigProvider.ConfigurationToXML(configuration), out results);
            if (results.IsValid)
            {
                MessageBox.Show("Configuration is valid!", "Validation");
            }
            else
            {
                MessageBox.Show(string.Format(
                    "Configuration is NOT valid!\nDetails:\n{0}",
                    string.Join("\n", results.ViolatedAssertions.Select(
                    (assertion) => assertion.UserMessage))), "Validation");
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedTab == xmlTabPage)
            {
                UpdateXml();
            }
        }

        private void UpdateXml()
        {
            FillConfigFromGUI();
            xmlConfiguration = ConfigProvider.ConfigurationToXML(configuration);
            xmlSourceTextBox.Text = xmlConfiguration.ToString();
        }

        private void serviceNameTextBox_TextChanged(object sender, EventArgs e)
        {
            bool renamed = RenameService(serviceNameTextBox.Text);
            serviceNameTextBox.BackColor = renamed ? Color.White : Color.IndianRed;
        }

        private bool RenameService(string newServiceName)
        {
            if (string.IsNullOrEmpty(newServiceName) ||
                ((newServiceName != currentServiceName) &&
                serviceNames.Contains(newServiceName)))
            {
                return false;
            }

            int index = serviceNames.IndexOf(currentServiceName);
            currentServiceName = newServiceName;
            currentService.Name = currentServiceName;
            serviceNames[index] = currentServiceName;
            RefreshListBox(servicesListBox);
            return true;
        }

        private void serviceTypeClassTextBox_TextChanged(object sender, EventArgs e)
        {
            if (currentService != null)
            {
                currentService.TypeClass = serviceTypeClassTextBox.Text;
            }
        }

        private void serviceTypeAssemblyTextBox_TextChanged(object sender, EventArgs e)
        {
            if (currentService != null)
            {
                currentService.TypeAssembly = serviceTypeAssemblyTextBox.Text;
            }
        }

        private void installerDescriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (currentService != null)
            {
                currentService.InstallerSettings.Description = installerDescriptionTextBox.Text;
            }
        }

        private void installerStartTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentService != null)
            {
                currentService.InstallerSettings.StartMode = installerStartTypeComboBox.Text;
            }
        }

        private void installerAccountComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentService != null)
            {
                currentService.InstallerSettings.Account = installerAccountComboBox.Text;
            }
        }

        private void installerUsernameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (currentService != null)
            {
                currentService.InstallerSettings.User = installerUsernameTextBox.Text;
            }
        }

        private void installerPasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            if (currentService != null)
            {
                currentService.InstallerSettings.Password = installerPasswordTextBox.Text;
            }
        }

        private void traceLoggerBufferSizeNumeric_ValueChanged(object sender, EventArgs e)
        {
            if (currentService != null)
            {
                currentService.TraceLoggerSettings.BufferSize = (int)traceLoggerBufferSizeNumeric.Value;
            }
        }

        private void addRequiredServiceButton_Click(object sender, EventArgs e)
        {
            string serviceName;
            if (!EditNameInForm(serviceNameForm, string.Empty, installerRequiredServiceNames, out serviceName))
            {
                return;
            }
            installerRequiredServiceNames.Add(serviceName);

            UpdateInstallerRequiredServices();
        }

        private void removeRequiredServiceButton_Click(object sender, EventArgs e)
        {
            if (installerRequiredServiceNames.Count() <= 0)
            {
                return;
            }
            string selectedName = (string)installerRequiredServicesListBox.SelectedItem;
            installerRequiredServiceNames.Remove(selectedName);

            UpdateInstallerRequiredServices();
        }

        private void editRequiredServiceButton_Click(object sender, EventArgs e)
        {
            if (installerRequiredServiceNames.Count() <= 0)
            {
                return;
            }
            string selectedName = (string)installerRequiredServicesListBox.SelectedItem;

            string newServiceName;
            if (!EditNameInForm(serviceNameForm, selectedName, installerRequiredServiceNames, out newServiceName))
            {
                return;
            }
            installerRequiredServiceNames.Remove(selectedName);
            installerRequiredServiceNames.Add(newServiceName);

            UpdateInstallerRequiredServices();
        }

        private void UpdateInstallerRequiredServices()
        {
            installerRequiredServiceNames.Sort();
            RefreshListBox(installerRequiredServicesListBox);
            if (currentService != null)
            {
                currentService.InstallerSettings.RequiredServices = installerRequiredServiceNames.ToArray();
            }
        }

        private void UpdateTraceLoggerStorages()
        {
            traceLoggerStorageNames.Sort();
            RefreshListBox(traceLoggerStoragesListBox);
        }

        private void addTraceLoggerStorageButton_Click(object sender, EventArgs e)
        {
            string name;
            if (!EditNameInForm(storageNameForm, string.Empty, traceLoggerStorageNames, out name))
            {
                return;
            }
            traceLoggerStorageNames.Add(name);

            UpdateTraceLoggerStorages();

            traceLoggerStoragesListBox.SelectedIndex = -1;
            if (currentService != null)
            {
                currentTraceLoggerStorage = new TraceLoggerStorageSettings() { Name = name };
                currentService.TraceLoggerSettings.Storages.Add(currentTraceLoggerStorage);
                traceLoggerStoragesListBox.SelectedIndex =
                    traceLoggerStorageNames.IndexOf(name);
            }
        }

        private void removeTraceLoggerStorageButton_Click(object sender, EventArgs e)
        {
            if (traceLoggerStorageNames.Count() <= 0)
            {
                return;
            }
            string selectedName = (string)traceLoggerStoragesListBox.SelectedItem;
            traceLoggerStorageNames.Remove(selectedName);

            UpdateTraceLoggerStorages();

            if (traceLoggerStorageNames.Count() <= 0)
            {
                traceLoggerStorageGroupBox.Enabled = false;
            }

            if (currentService != null)
            {
                currentService.TraceLoggerSettings.Storages.RemoveAll(
                    storage => storage.Name == selectedName);
            }

            traceLoggerStoragesListBox.SelectedIndex = -1;
            currentTraceLoggerStorage = null;
            FillTraceLoggerStorageToGUI(null);
            traceLoggerStorageGroupBox.Enabled = false;
        }

        private void traceLoggerStorageClassTextBox_TextChanged(object sender, EventArgs e)
        {
            if (currentTraceLoggerStorage != null)
            {
                currentTraceLoggerStorage.TypeClass = traceLoggerStorageClassTextBox.Text;
            }
        }

        private void traceLoggerStorageAssemblyTextBox_TextChanged(object sender, EventArgs e)
        {
            if (currentTraceLoggerStorage != null)
            {
                currentTraceLoggerStorage.TypeAssembly = traceLoggerStorageAssemblyTextBox.Text;
            }
        }

        private void editTraceLoggerStorageButton_Click(object sender, EventArgs e)
        {
            if (traceLoggerStorageNames.Count() <= 0)
            {
                return;
            }
            string selectedName = (string)traceLoggerStoragesListBox.SelectedItem;

            string newServiceName;
            if (!EditNameInForm(storageNameForm, selectedName, traceLoggerStorageNames, out newServiceName))
            {
                return;
            }
            traceLoggerStorageNames.Remove(selectedName);
            traceLoggerStorageNames.Add(newServiceName);
            currentTraceLoggerStorageName = newServiceName;
            currentTraceLoggerStorage.Name = newServiceName;

            UpdateTraceLoggerStorages();
            traceLoggerStoragesListBox.SelectedIndex = -1;
            traceLoggerStoragesListBox.SelectedIndex =
                traceLoggerStorageNames.IndexOf(currentTraceLoggerStorageName);
        }
    }
}
