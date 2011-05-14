using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DaemonNT.Configuration;

namespace DaemonNT.GUI.ConfigEditor
{
    public partial class SettingsEditorForm : Form
    {
        public Settings Settings { get; set; }

        public SettingsEditorForm()
        {
            InitializeComponent();
        }

        #region Event handlers

        private void SettingsEditorForm_Load(object sender, EventArgs e)
        {
            ClearGUI();
            FillSettingsToGUI(Settings);
        }

        private void SettingsEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                Settings = FillSettingsFromGUI();
            }
        }

        #endregion

        #region Helper methods - form logic

        private Settings FillSettingsFromGUI()
        {
            return new Settings();
        }

        private void FillSettingsToGUI(Settings settings)
        {
            TreeNode root = sectionsTreeView.Nodes.Add("root settings");
            root.Tag = settings;
            if (settings != null)
            {
                FillSection(root, settings);
            }
            sectionsTreeView.SelectedNode = root;
            sectionsTreeView.ExpandAll();
        }

        private void FillSection(TreeNode root, SectionBase section)
        {
            foreach (string subSectionName in section.Keys)
            {
                SectionBase subSection = section[subSectionName];
                if (subSection != null)
                {
                    TreeNode node = root.Nodes.Add(subSectionName);
                    node.Tag = subSection;
                    FillSection(node, subSection);
                }
            }
        }

        private void FillParameters(Parameters parameters)
        {
            parametersListView.Items.Clear();
            foreach (string key in parameters.Keys)
            {
                string value = parameters[key];
                ListViewItem item = new ListViewItem(new[] { key, value });
                parametersListView.Items.Add(item);
            }
        }

        private void ClearGUI()
        {
            sectionsTreeView.Nodes.Clear();
            parametersListView.Items.Clear();
        }

        #endregion

        private void sectionsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode sectionNode = sectionsTreeView.SelectedNode;
            if (sectionNode != null)
            {
                SectionBase section = (SectionBase)sectionNode.Tag;
                if (section != null)
                {
                    FillParameters(section.Parameters);
                }
            }
        }
    }
}
