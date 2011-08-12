namespace DaemonNT.GUI.ConfigEditor
{
    partial class SettingsEditorForm
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.sectionsGroupBox = new System.Windows.Forms.GroupBox();
            this.sectionNameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.sectionsTreeView = new System.Windows.Forms.TreeView();
            this.parametersGroupBox = new System.Windows.Forms.GroupBox();
            this.paramValueTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.paramNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.parametersListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.parameterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addParamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editParamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeParamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.sectionsGroupBox.SuspendLayout();
            this.parametersGroupBox.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(482, 422);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(401, 422);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 6;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 27);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.sectionsGroupBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.parametersGroupBox);
            this.splitContainer1.Size = new System.Drawing.Size(545, 389);
            this.splitContainer1.SplitterDistance = 181;
            this.splitContainer1.TabIndex = 3;
            // 
            // sectionsGroupBox
            // 
            this.sectionsGroupBox.Controls.Add(this.sectionNameTextBox);
            this.sectionsGroupBox.Controls.Add(this.label3);
            this.sectionsGroupBox.Controls.Add(this.sectionsTreeView);
            this.sectionsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sectionsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.sectionsGroupBox.Name = "sectionsGroupBox";
            this.sectionsGroupBox.Size = new System.Drawing.Size(181, 389);
            this.sectionsGroupBox.TabIndex = 3;
            this.sectionsGroupBox.TabStop = false;
            this.sectionsGroupBox.Text = "Sections";
            // 
            // sectionNameTextBox
            // 
            this.sectionNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sectionNameTextBox.Location = new System.Drawing.Point(50, 16);
            this.sectionNameTextBox.Name = "sectionNameTextBox";
            this.sectionNameTextBox.Size = new System.Drawing.Size(125, 20);
            this.sectionNameTextBox.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Name:";
            // 
            // sectionsTreeView
            // 
            this.sectionsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sectionsTreeView.Location = new System.Drawing.Point(6, 45);
            this.sectionsTreeView.Name = "sectionsTreeView";
            this.sectionsTreeView.Size = new System.Drawing.Size(169, 338);
            this.sectionsTreeView.TabIndex = 2;
            this.sectionsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.sectionsTreeView_AfterSelect);
            // 
            // parametersGroupBox
            // 
            this.parametersGroupBox.Controls.Add(this.paramValueTextBox);
            this.parametersGroupBox.Controls.Add(this.label2);
            this.parametersGroupBox.Controls.Add(this.paramNameTextBox);
            this.parametersGroupBox.Controls.Add(this.label1);
            this.parametersGroupBox.Controls.Add(this.parametersListView);
            this.parametersGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parametersGroupBox.Location = new System.Drawing.Point(0, 0);
            this.parametersGroupBox.Name = "parametersGroupBox";
            this.parametersGroupBox.Size = new System.Drawing.Size(360, 389);
            this.parametersGroupBox.TabIndex = 0;
            this.parametersGroupBox.TabStop = false;
            this.parametersGroupBox.Text = "Parameters";
            // 
            // paramValueTextBox
            // 
            this.paramValueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.paramValueTextBox.Location = new System.Drawing.Point(50, 42);
            this.paramValueTextBox.Name = "paramValueTextBox";
            this.paramValueTextBox.Size = new System.Drawing.Size(304, 20);
            this.paramValueTextBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Value:";
            // 
            // paramNameTextBox
            // 
            this.paramNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.paramNameTextBox.Location = new System.Drawing.Point(50, 16);
            this.paramNameTextBox.Name = "paramNameTextBox";
            this.paramNameTextBox.Size = new System.Drawing.Size(304, 20);
            this.paramNameTextBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // parametersListView
            // 
            this.parametersListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.parametersListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.parametersListView.Location = new System.Drawing.Point(6, 68);
            this.parametersListView.MultiSelect = false;
            this.parametersListView.Name = "parametersListView";
            this.parametersListView.Size = new System.Drawing.Size(348, 315);
            this.parametersListView.TabIndex = 5;
            this.parametersListView.UseCompatibleStateImageBehavior = false;
            this.parametersListView.View = System.Windows.Forms.View.Details;
            this.parametersListView.SelectedIndexChanged += new System.EventHandler(this.parametersListView_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 200;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.sectionToolStripMenuItem,
            this.parameterToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(569, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.cancelToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "&Settings";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // cancelToolStripMenuItem
            // 
            this.cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
            this.cancelToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.cancelToolStripMenuItem.Text = "&Cancel";
            this.cancelToolStripMenuItem.Click += new System.EventHandler(this.cancelToolStripMenuItem_Click);
            // 
            // sectionToolStripMenuItem
            // 
            this.sectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSectionToolStripMenuItem,
            this.editSectionToolStripMenuItem,
            this.removeSectionToolStripMenuItem});
            this.sectionToolStripMenuItem.Name = "sectionToolStripMenuItem";
            this.sectionToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.sectionToolStripMenuItem.Text = "Se&ction";
            // 
            // addSectionToolStripMenuItem
            // 
            this.addSectionToolStripMenuItem.Name = "addSectionToolStripMenuItem";
            this.addSectionToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addSectionToolStripMenuItem.Text = "&Add section";
            this.addSectionToolStripMenuItem.Click += new System.EventHandler(this.addSectionToolStripMenuItem_Click);
            // 
            // editSectionToolStripMenuItem
            // 
            this.editSectionToolStripMenuItem.Name = "editSectionToolStripMenuItem";
            this.editSectionToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.editSectionToolStripMenuItem.Text = "&Edit";
            this.editSectionToolStripMenuItem.Click += new System.EventHandler(this.editSectionToolStripMenuItem_Click);
            // 
            // removeSectionToolStripMenuItem
            // 
            this.removeSectionToolStripMenuItem.Name = "removeSectionToolStripMenuItem";
            this.removeSectionToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.removeSectionToolStripMenuItem.Text = "&Remove";
            this.removeSectionToolStripMenuItem.Click += new System.EventHandler(this.removeSectionToolStripMenuItem_Click);
            // 
            // parameterToolStripMenuItem
            // 
            this.parameterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addParamToolStripMenuItem,
            this.editParamToolStripMenuItem,
            this.removeParamToolStripMenuItem});
            this.parameterToolStripMenuItem.Name = "parameterToolStripMenuItem";
            this.parameterToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
            this.parameterToolStripMenuItem.Text = "&Parameter";
            // 
            // addParamToolStripMenuItem
            // 
            this.addParamToolStripMenuItem.Name = "addParamToolStripMenuItem";
            this.addParamToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.addParamToolStripMenuItem.Text = "&Add";
            this.addParamToolStripMenuItem.Click += new System.EventHandler(this.addParamToolStripMenuItem_Click);
            // 
            // editParamToolStripMenuItem
            // 
            this.editParamToolStripMenuItem.Name = "editParamToolStripMenuItem";
            this.editParamToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.editParamToolStripMenuItem.Text = "&Edit";
            this.editParamToolStripMenuItem.Click += new System.EventHandler(this.editParamToolStripMenuItem_Click);
            // 
            // removeParamToolStripMenuItem
            // 
            this.removeParamToolStripMenuItem.Name = "removeParamToolStripMenuItem";
            this.removeParamToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.removeParamToolStripMenuItem.Text = "&Remove";
            this.removeParamToolStripMenuItem.Click += new System.EventHandler(this.removeParamToolStripMenuItem_Click);
            // 
            // SettingsEditorForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(569, 457);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SettingsEditorForm";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsEditorForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.sectionsGroupBox.ResumeLayout(false);
            this.sectionsGroupBox.PerformLayout();
            this.parametersGroupBox.ResumeLayout(false);
            this.parametersGroupBox.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox sectionsGroupBox;
        private System.Windows.Forms.TreeView sectionsTreeView;
        private System.Windows.Forms.GroupBox parametersGroupBox;
        private System.Windows.Forms.ListView parametersListView;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem parameterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addParamToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editParamToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeParamToolStripMenuItem;
        private System.Windows.Forms.TextBox paramValueTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox paramNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox sectionNameTextBox;
        private System.Windows.Forms.Label label3;
    }
}