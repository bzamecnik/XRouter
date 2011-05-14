namespace DaemonNT.GUI.ConfigEditor
{
    partial class ConfigEditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.objectModelTabPage = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.servicesListBox = new System.Windows.Forms.ListBox();
            this.serviceGroupBox = new System.Windows.Forms.GroupBox();
            this.traceLoggerGroupBox = new System.Windows.Forms.GroupBox();
            this.traceLoggerStoragesListBox = new System.Windows.Forms.ListBox();
            this.traceLoggerStorageGroupBox = new System.Windows.Forms.GroupBox();
            this.editStorageSettingsButton = new System.Windows.Forms.Button();
            this.traceLoggerStorageClassTextBox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.traceLoggerStorageNameTextBox = new System.Windows.Forms.TextBox();
            this.traceLoggerStorageAssemblyTextBox = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.traceLoggerBufferSizeNumeric = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.settingsGroupBox = new System.Windows.Forms.GroupBox();
            this.editServiceSettingsButton = new System.Windows.Forms.Button();
            this.installerGroupBox = new System.Windows.Forms.GroupBox();
            this.installerRequiredServicesListBox = new System.Windows.Forms.ListBox();
            this.installerAccountComboBox = new System.Windows.Forms.ComboBox();
            this.accountLabel = new System.Windows.Forms.Label();
            this.installerStartTypeComboBox = new System.Windows.Forms.ComboBox();
            this.startTypeLabel = new System.Windows.Forms.Label();
            this.installerPasswordTextBox = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.requiredServicesLabel = new System.Windows.Forms.Label();
            this.installerUsernameTextBox = new System.Windows.Forms.TextBox();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.installerDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.serviceTypeAssemblyTextBox = new System.Windows.Forms.TextBox();
            this.serviceTypeClassTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.serviceNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.xmlTabPage = new System.Windows.Forms.TabPage();
            this.xmlSourceTextBox = new System.Windows.Forms.TextBox();
            this.openConfigFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveConfigFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.objectModelTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.serviceGroupBox.SuspendLayout();
            this.traceLoggerGroupBox.SuspendLayout();
            this.traceLoggerStorageGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.traceLoggerBufferSizeNumeric)).BeginInit();
            this.settingsGroupBox.SuspendLayout();
            this.installerGroupBox.SuspendLayout();
            this.xmlTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.serviceToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(753, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItem1.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.saveAsToolStripMenuItem.Text = "Save &as";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.closeToolStripMenuItem.Text = "Close";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(181, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // serviceToolStripMenuItem
            // 
            this.serviceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.serviceToolStripMenuItem.Name = "serviceToolStripMenuItem";
            this.serviceToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.serviceToolStripMenuItem.Text = "&Service";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 610);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(753, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.objectModelTabPage);
            this.tabControl1.Controls.Add(this.xmlTabPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(753, 586);
            this.tabControl1.TabIndex = 2;
            // 
            // objectModelTabPage
            // 
            this.objectModelTabPage.Controls.Add(this.splitContainer1);
            this.objectModelTabPage.Location = new System.Drawing.Point(4, 22);
            this.objectModelTabPage.Name = "objectModelTabPage";
            this.objectModelTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.objectModelTabPage.Size = new System.Drawing.Size(745, 560);
            this.objectModelTabPage.TabIndex = 0;
            this.objectModelTabPage.Text = "Object model";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.servicesListBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.serviceGroupBox);
            this.splitContainer1.Size = new System.Drawing.Size(739, 554);
            this.splitContainer1.SplitterDistance = 150;
            this.splitContainer1.TabIndex = 0;
            // 
            // servicesListBox
            // 
            this.servicesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.servicesListBox.FormattingEnabled = true;
            this.servicesListBox.Location = new System.Drawing.Point(0, 0);
            this.servicesListBox.Name = "servicesListBox";
            this.servicesListBox.Size = new System.Drawing.Size(150, 554);
            this.servicesListBox.TabIndex = 0;
            this.servicesListBox.SelectedIndexChanged += new System.EventHandler(this.servicesListBox_SelectedIndexChanged);
            // 
            // serviceGroupBox
            // 
            this.serviceGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serviceGroupBox.Controls.Add(this.traceLoggerGroupBox);
            this.serviceGroupBox.Controls.Add(this.settingsGroupBox);
            this.serviceGroupBox.Controls.Add(this.installerGroupBox);
            this.serviceGroupBox.Controls.Add(this.serviceTypeAssemblyTextBox);
            this.serviceGroupBox.Controls.Add(this.serviceTypeClassTextBox);
            this.serviceGroupBox.Controls.Add(this.label3);
            this.serviceGroupBox.Controls.Add(this.label2);
            this.serviceGroupBox.Controls.Add(this.serviceNameTextBox);
            this.serviceGroupBox.Controls.Add(this.label1);
            this.serviceGroupBox.Location = new System.Drawing.Point(3, 3);
            this.serviceGroupBox.Name = "serviceGroupBox";
            this.serviceGroupBox.Size = new System.Drawing.Size(579, 548);
            this.serviceGroupBox.TabIndex = 0;
            this.serviceGroupBox.TabStop = false;
            this.serviceGroupBox.Text = "Service";
            // 
            // traceLoggerGroupBox
            // 
            this.traceLoggerGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.traceLoggerGroupBox.Controls.Add(this.traceLoggerStoragesListBox);
            this.traceLoggerGroupBox.Controls.Add(this.traceLoggerStorageGroupBox);
            this.traceLoggerGroupBox.Controls.Add(this.label11);
            this.traceLoggerGroupBox.Controls.Add(this.traceLoggerBufferSizeNumeric);
            this.traceLoggerGroupBox.Controls.Add(this.label10);
            this.traceLoggerGroupBox.Location = new System.Drawing.Point(6, 313);
            this.traceLoggerGroupBox.Name = "traceLoggerGroupBox";
            this.traceLoggerGroupBox.Size = new System.Drawing.Size(567, 160);
            this.traceLoggerGroupBox.TabIndex = 8;
            this.traceLoggerGroupBox.TabStop = false;
            this.traceLoggerGroupBox.Text = "Trace logger";
            // 
            // traceLoggerStoragesListBox
            // 
            this.traceLoggerStoragesListBox.FormattingEnabled = true;
            this.traceLoggerStoragesListBox.Location = new System.Drawing.Point(71, 50);
            this.traceLoggerStoragesListBox.Name = "traceLoggerStoragesListBox";
            this.traceLoggerStoragesListBox.Size = new System.Drawing.Size(172, 95);
            this.traceLoggerStoragesListBox.TabIndex = 5;
            this.traceLoggerStoragesListBox.SelectedIndexChanged += new System.EventHandler(this.traceLoggerStoragesListBox_SelectedIndexChanged);
            // 
            // traceLoggerStorageGroupBox
            // 
            this.traceLoggerStorageGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.traceLoggerStorageGroupBox.Controls.Add(this.editStorageSettingsButton);
            this.traceLoggerStorageGroupBox.Controls.Add(this.traceLoggerStorageClassTextBox);
            this.traceLoggerStorageGroupBox.Controls.Add(this.label12);
            this.traceLoggerStorageGroupBox.Controls.Add(this.traceLoggerStorageNameTextBox);
            this.traceLoggerStorageGroupBox.Controls.Add(this.traceLoggerStorageAssemblyTextBox);
            this.traceLoggerStorageGroupBox.Controls.Add(this.label13);
            this.traceLoggerStorageGroupBox.Controls.Add(this.label14);
            this.traceLoggerStorageGroupBox.Location = new System.Drawing.Point(249, 19);
            this.traceLoggerStorageGroupBox.Name = "traceLoggerStorageGroupBox";
            this.traceLoggerStorageGroupBox.Size = new System.Drawing.Size(312, 135);
            this.traceLoggerStorageGroupBox.TabIndex = 4;
            this.traceLoggerStorageGroupBox.TabStop = false;
            this.traceLoggerStorageGroupBox.Text = "Storage";
            // 
            // editStorageSettingsButton
            // 
            this.editStorageSettingsButton.Location = new System.Drawing.Point(6, 97);
            this.editStorageSettingsButton.Name = "editStorageSettingsButton";
            this.editStorageSettingsButton.Size = new System.Drawing.Size(112, 23);
            this.editStorageSettingsButton.TabIndex = 0;
            this.editStorageSettingsButton.Text = "Edit storage settings";
            this.editStorageSettingsButton.UseVisualStyleBackColor = true;
            this.editStorageSettingsButton.Click += new System.EventHandler(this.editStorageSettingsButton_Click);
            // 
            // traceLoggerStorageClassTextBox
            // 
            this.traceLoggerStorageClassTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.traceLoggerStorageClassTextBox.Location = new System.Drawing.Point(72, 44);
            this.traceLoggerStorageClassTextBox.Name = "traceLoggerStorageClassTextBox";
            this.traceLoggerStorageClassTextBox.Size = new System.Drawing.Size(234, 20);
            this.traceLoggerStorageClassTextBox.TabIndex = 4;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(11, 20);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(38, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Name:";
            // 
            // traceLoggerStorageNameTextBox
            // 
            this.traceLoggerStorageNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.traceLoggerStorageNameTextBox.Location = new System.Drawing.Point(72, 17);
            this.traceLoggerStorageNameTextBox.Name = "traceLoggerStorageNameTextBox";
            this.traceLoggerStorageNameTextBox.Size = new System.Drawing.Size(234, 20);
            this.traceLoggerStorageNameTextBox.TabIndex = 1;
            // 
            // traceLoggerStorageAssemblyTextBox
            // 
            this.traceLoggerStorageAssemblyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.traceLoggerStorageAssemblyTextBox.Location = new System.Drawing.Point(72, 71);
            this.traceLoggerStorageAssemblyTextBox.Name = "traceLoggerStorageAssemblyTextBox";
            this.traceLoggerStorageAssemblyTextBox.Size = new System.Drawing.Size(234, 20);
            this.traceLoggerStorageAssemblyTextBox.TabIndex = 5;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(11, 47);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(35, 13);
            this.label13.TabIndex = 2;
            this.label13.Text = "Class:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(10, 74);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(54, 13);
            this.label14.TabIndex = 3;
            this.label14.Text = "Assembly:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 50);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(52, 13);
            this.label11.TabIndex = 3;
            this.label11.Text = "Storages:";
            // 
            // traceLoggerBufferSizeNumeric
            // 
            this.traceLoggerBufferSizeNumeric.Location = new System.Drawing.Point(88, 19);
            this.traceLoggerBufferSizeNumeric.Maximum = new decimal(new int[] {
            -727379968,
            232,
            0,
            0});
            this.traceLoggerBufferSizeNumeric.Name = "traceLoggerBufferSizeNumeric";
            this.traceLoggerBufferSizeNumeric.Size = new System.Drawing.Size(121, 20);
            this.traceLoggerBufferSizeNumeric.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 21);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Buffer size:";
            // 
            // settingsGroupBox
            // 
            this.settingsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsGroupBox.Controls.Add(this.editServiceSettingsButton);
            this.settingsGroupBox.Location = new System.Drawing.Point(6, 252);
            this.settingsGroupBox.Name = "settingsGroupBox";
            this.settingsGroupBox.Size = new System.Drawing.Size(567, 55);
            this.settingsGroupBox.TabIndex = 7;
            this.settingsGroupBox.TabStop = false;
            this.settingsGroupBox.Text = "Settings";
            // 
            // editServiceSettingsButton
            // 
            this.editServiceSettingsButton.Location = new System.Drawing.Point(9, 19);
            this.editServiceSettingsButton.Name = "editServiceSettingsButton";
            this.editServiceSettingsButton.Size = new System.Drawing.Size(112, 23);
            this.editServiceSettingsButton.TabIndex = 0;
            this.editServiceSettingsButton.Text = "Edit service settings";
            this.editServiceSettingsButton.UseVisualStyleBackColor = true;
            this.editServiceSettingsButton.Click += new System.EventHandler(this.editServiceSettingsButton_Click);
            // 
            // installerGroupBox
            // 
            this.installerGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.installerGroupBox.Controls.Add(this.installerRequiredServicesListBox);
            this.installerGroupBox.Controls.Add(this.installerAccountComboBox);
            this.installerGroupBox.Controls.Add(this.accountLabel);
            this.installerGroupBox.Controls.Add(this.installerStartTypeComboBox);
            this.installerGroupBox.Controls.Add(this.startTypeLabel);
            this.installerGroupBox.Controls.Add(this.installerPasswordTextBox);
            this.installerGroupBox.Controls.Add(this.passwordLabel);
            this.installerGroupBox.Controls.Add(this.requiredServicesLabel);
            this.installerGroupBox.Controls.Add(this.installerUsernameTextBox);
            this.installerGroupBox.Controls.Add(this.usernameLabel);
            this.installerGroupBox.Controls.Add(this.installerDescriptionTextBox);
            this.installerGroupBox.Controls.Add(this.descriptionLabel);
            this.installerGroupBox.Location = new System.Drawing.Point(7, 97);
            this.installerGroupBox.Name = "installerGroupBox";
            this.installerGroupBox.Size = new System.Drawing.Size(566, 148);
            this.installerGroupBox.TabIndex = 6;
            this.installerGroupBox.TabStop = false;
            this.installerGroupBox.Text = "Installer";
            // 
            // installerRequiredServicesListBox
            // 
            this.installerRequiredServicesListBox.FormattingEnabled = true;
            this.installerRequiredServicesListBox.Location = new System.Drawing.Point(237, 65);
            this.installerRequiredServicesListBox.Name = "installerRequiredServicesListBox";
            this.installerRequiredServicesListBox.Size = new System.Drawing.Size(227, 69);
            this.installerRequiredServicesListBox.TabIndex = 3;
            // 
            // installerAccountComboBox
            // 
            this.installerAccountComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.installerAccountComboBox.FormattingEnabled = true;
            this.installerAccountComboBox.Location = new System.Drawing.Point(87, 66);
            this.installerAccountComboBox.Name = "installerAccountComboBox";
            this.installerAccountComboBox.Size = new System.Drawing.Size(121, 21);
            this.installerAccountComboBox.TabIndex = 2;
            // 
            // accountLabel
            // 
            this.accountLabel.AutoSize = true;
            this.accountLabel.Location = new System.Drawing.Point(6, 69);
            this.accountLabel.Name = "accountLabel";
            this.accountLabel.Size = new System.Drawing.Size(50, 13);
            this.accountLabel.TabIndex = 0;
            this.accountLabel.Text = "Account:";
            // 
            // installerStartTypeComboBox
            // 
            this.installerStartTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.installerStartTypeComboBox.FormattingEnabled = true;
            this.installerStartTypeComboBox.Location = new System.Drawing.Point(87, 39);
            this.installerStartTypeComboBox.Name = "installerStartTypeComboBox";
            this.installerStartTypeComboBox.Size = new System.Drawing.Size(121, 21);
            this.installerStartTypeComboBox.TabIndex = 2;
            // 
            // startTypeLabel
            // 
            this.startTypeLabel.AutoSize = true;
            this.startTypeLabel.Location = new System.Drawing.Point(6, 42);
            this.startTypeLabel.Name = "startTypeLabel";
            this.startTypeLabel.Size = new System.Drawing.Size(55, 13);
            this.startTypeLabel.TabIndex = 0;
            this.startTypeLabel.Text = "Start type:";
            // 
            // installerPasswordTextBox
            // 
            this.installerPasswordTextBox.Location = new System.Drawing.Point(87, 119);
            this.installerPasswordTextBox.Name = "installerPasswordTextBox";
            this.installerPasswordTextBox.PasswordChar = '*';
            this.installerPasswordTextBox.Size = new System.Drawing.Size(121, 20);
            this.installerPasswordTextBox.TabIndex = 1;
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(6, 122);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(56, 13);
            this.passwordLabel.TabIndex = 0;
            this.passwordLabel.Text = "Password:";
            // 
            // requiredServicesLabel
            // 
            this.requiredServicesLabel.AutoSize = true;
            this.requiredServicesLabel.Location = new System.Drawing.Point(234, 42);
            this.requiredServicesLabel.Name = "requiredServicesLabel";
            this.requiredServicesLabel.Size = new System.Drawing.Size(95, 13);
            this.requiredServicesLabel.TabIndex = 0;
            this.requiredServicesLabel.Text = "Required services:";
            // 
            // installerUsernameTextBox
            // 
            this.installerUsernameTextBox.Location = new System.Drawing.Point(87, 93);
            this.installerUsernameTextBox.Name = "installerUsernameTextBox";
            this.installerUsernameTextBox.Size = new System.Drawing.Size(121, 20);
            this.installerUsernameTextBox.TabIndex = 1;
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Location = new System.Drawing.Point(6, 96);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(61, 13);
            this.usernameLabel.TabIndex = 0;
            this.usernameLabel.Text = "User name:";
            // 
            // installerDescriptionTextBox
            // 
            this.installerDescriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.installerDescriptionTextBox.Location = new System.Drawing.Point(87, 13);
            this.installerDescriptionTextBox.Name = "installerDescriptionTextBox";
            this.installerDescriptionTextBox.Size = new System.Drawing.Size(473, 20);
            this.installerDescriptionTextBox.TabIndex = 1;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Location = new System.Drawing.Point(6, 16);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(63, 13);
            this.descriptionLabel.TabIndex = 0;
            this.descriptionLabel.Text = "Description:";
            // 
            // serviceTypeAssemblyTextBox
            // 
            this.serviceTypeAssemblyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serviceTypeAssemblyTextBox.Location = new System.Drawing.Point(88, 71);
            this.serviceTypeAssemblyTextBox.Name = "serviceTypeAssemblyTextBox";
            this.serviceTypeAssemblyTextBox.Size = new System.Drawing.Size(479, 20);
            this.serviceTypeAssemblyTextBox.TabIndex = 5;
            // 
            // serviceTypeClassTextBox
            // 
            this.serviceTypeClassTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serviceTypeClassTextBox.Location = new System.Drawing.Point(88, 44);
            this.serviceTypeClassTextBox.Name = "serviceTypeClassTextBox";
            this.serviceTypeClassTextBox.Size = new System.Drawing.Size(479, 20);
            this.serviceTypeClassTextBox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Assembly:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Class:";
            // 
            // serviceNameTextBox
            // 
            this.serviceNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serviceNameTextBox.Location = new System.Drawing.Point(88, 17);
            this.serviceNameTextBox.Name = "serviceNameTextBox";
            this.serviceNameTextBox.Size = new System.Drawing.Size(479, 20);
            this.serviceNameTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Service name:";
            // 
            // xmlTabPage
            // 
            this.xmlTabPage.Controls.Add(this.xmlSourceTextBox);
            this.xmlTabPage.Location = new System.Drawing.Point(4, 22);
            this.xmlTabPage.Name = "xmlTabPage";
            this.xmlTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.xmlTabPage.Size = new System.Drawing.Size(745, 560);
            this.xmlTabPage.TabIndex = 1;
            this.xmlTabPage.Text = "XML Source Code";
            // 
            // xmlSourceTextBox
            // 
            this.xmlSourceTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xmlSourceTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.xmlSourceTextBox.Location = new System.Drawing.Point(3, 3);
            this.xmlSourceTextBox.Multiline = true;
            this.xmlSourceTextBox.Name = "xmlSourceTextBox";
            this.xmlSourceTextBox.ReadOnly = true;
            this.xmlSourceTextBox.Size = new System.Drawing.Size(739, 554);
            this.xmlSourceTextBox.TabIndex = 0;
            // 
            // openConfigFileDialog
            // 
            this.openConfigFileDialog.Filter = "XML files|*.xml|All files|*.*";
            this.openConfigFileDialog.Title = "Open configuration file";
            // 
            // saveConfigFileDialog
            // 
            this.saveConfigFileDialog.Filter = "XML files|*.xml|All files|*.*";
            this.saveConfigFileDialog.Title = "Save configuration file";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newToolStripMenuItem.Text = "&New";
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.removeToolStripMenuItem.Text = "&Remove";
            // 
            // ConfigEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 632);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ConfigEditorForm";
            this.Text = "DaemonNT Configuration File Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.objectModelTabPage.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.serviceGroupBox.ResumeLayout(false);
            this.serviceGroupBox.PerformLayout();
            this.traceLoggerGroupBox.ResumeLayout(false);
            this.traceLoggerGroupBox.PerformLayout();
            this.traceLoggerStorageGroupBox.ResumeLayout(false);
            this.traceLoggerStorageGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.traceLoggerBufferSizeNumeric)).EndInit();
            this.settingsGroupBox.ResumeLayout(false);
            this.installerGroupBox.ResumeLayout(false);
            this.installerGroupBox.PerformLayout();
            this.xmlTabPage.ResumeLayout(false);
            this.xmlTabPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage objectModelTabPage;
        private System.Windows.Forms.TabPage xmlTabPage;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox serviceGroupBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox serviceNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox installerGroupBox;
        private System.Windows.Forms.TextBox serviceTypeAssemblyTextBox;
        private System.Windows.Forms.TextBox serviceTypeClassTextBox;
        private System.Windows.Forms.GroupBox settingsGroupBox;
        private System.Windows.Forms.ComboBox installerAccountComboBox;
        private System.Windows.Forms.Label accountLabel;
        private System.Windows.Forms.ComboBox installerStartTypeComboBox;
        private System.Windows.Forms.Label startTypeLabel;
        private System.Windows.Forms.TextBox installerPasswordTextBox;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.Label requiredServicesLabel;
        private System.Windows.Forms.TextBox installerUsernameTextBox;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.TextBox installerDescriptionTextBox;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.GroupBox traceLoggerGroupBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown traceLoggerBufferSizeNumeric;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button editServiceSettingsButton;
        private System.Windows.Forms.GroupBox traceLoggerStorageGroupBox;
        private System.Windows.Forms.Button editStorageSettingsButton;
        private System.Windows.Forms.TextBox traceLoggerStorageClassTextBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox traceLoggerStorageNameTextBox;
        private System.Windows.Forms.TextBox traceLoggerStorageAssemblyTextBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox xmlSourceTextBox;
        private System.Windows.Forms.OpenFileDialog openConfigFileDialog;
        private System.Windows.Forms.SaveFileDialog saveConfigFileDialog;
        private System.Windows.Forms.ToolStripMenuItem serviceToolStripMenuItem;
        private System.Windows.Forms.ListBox installerRequiredServicesListBox;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ListBox servicesListBox;
        private System.Windows.Forms.ListBox traceLoggerStoragesListBox;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
    }
}

