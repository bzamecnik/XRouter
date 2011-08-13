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

        private string editedParamName;

        public SettingsEditorForm()
        {
            InitializeComponent();
        }

        #region Event handlers

        private void SettingsEditorForm_Load(object sender, EventArgs e)
        {
            editedParamName = string.Empty;
            ClearGUI();
            if (Settings == null)
            {
                Settings = new Settings();
            }
            FillSettingsToGUI(Settings);
        }

        private void SettingsEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
            //{
            //    Settings = FillSettingsFromGUI();
            //}
        }

        #endregion

        #region Helper methods - form logic

        //private Settings FillSettingsFromGUI()
        //{
        //    return new Settings();
        //}

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
                parametersListView.Items.Add(key, value, 0);
            }
        }

        private void ClearGUI()
        {
            sectionsTreeView.Nodes.Clear();
            parametersListView.Items.Clear();
            sectionNameTextBox.Text = string.Empty;
            paramNameTextBox.Text = string.Empty;
            paramValueTextBox.Text = string.Empty;
        }

        #endregion

        private void sectionsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SectionBase section = GetSelectedSection();
            if (section != null)
            {
                FillParameters(section.Parameters);
            }
            editedParamName = string.Empty;
            paramNameTextBox.Text = string.Empty;
            paramValueTextBox.Text = string.Empty;
        }

        private SectionBase GetSelectedSection()
        {
            TreeNode sectionNode = sectionsTreeView.SelectedNode;
            if(sectionNode != null) {
                return (SectionBase)sectionNode.Tag;
            } else {
                return Settings;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void addParamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var section = GetSelectedSection();
            string key = paramNameTextBox.Text;
            if (!string.IsNullOrEmpty(key) && !section.Parameters.Keys.Contains(key))
            {
                string value = paramValueTextBox.Text;
                parametersListView.Items.Add(key, value, 0);
                section.Parameters[key] = value;
            }
            editedParamName = key;
        }

        private void removeSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var section = GetSelectedSection();
            string key = paramNameTextBox.Text;
            if (!string.IsNullOrEmpty(key))
            {
                parametersListView.Items.RemoveByKey(key);
                section.Parameters.Remove(key);
            }
        }

        private void parametersListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ParamToGui();
        }

        private void ParamToGui()
        {
            var section = GetSelectedSection();
            if (parametersListView.SelectedIndices.Count > 0)
            {
                var paramName = section.Parameters.Keys.ElementAt(parametersListView.SelectedIndices[0]);
                editedParamName = (string)paramName.Clone();
                paramNameTextBox.Text = paramName;
                paramValueTextBox.Text = section.Parameters[paramName];
            }
            else
            {
                editedParamName = string.Empty;
                paramNameTextBox.Text = string.Empty;
                paramValueTextBox.Text = string.Empty;
            }
        }

        private void editParamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string key = paramNameTextBox.Text;
            var section = GetSelectedSection();
            
            if (!string.IsNullOrEmpty(key))
            {
                string value = paramValueTextBox.Text;
                int index = parametersListView.Items.IndexOfKey(editedParamName);
                if (index > 0)
                {
                    parametersListView.Items[index].Text = value;
                }
                section.Parameters[key] = value;
                if (!section.Parameters.Keys.Contains(key))
                {
                    section.Parameters.Remove(editedParamName);
                }
            }
            editedParamName = (string)key.Clone();
        }
    }
}
