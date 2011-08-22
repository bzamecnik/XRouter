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

        #endregion

        #region Helper methods - form logic

        //private Settings FillSettingsFromGUI()
        //{
        //    return new Settings();
        //}

        private void FillSettingsToGUI(Settings settings)
        {
            sectionsTreeView.BeginUpdate();
            TreeNode root = sectionsTreeView.Nodes.Add("(root settings)");
            root.Tag = settings;
            if (settings != null)
            {
                FillSection(root, settings);
            }
            sectionsTreeView.SelectedNode = root;
            sectionsTreeView.ExpandAll();
            sectionsTreeView.EndUpdate();
        }

        private void FillSection(TreeNode root, SectionBase section)
        {
            foreach (string subSectionName in section.Keys)
            {
                SectionBase subSection = section[subSectionName];
                if (subSection != null)
                {
                    TreeNode node = root.Nodes.Add(subSectionName);
                    node.Name = subSectionName;
                    node.Tag = subSection;
                    FillSection(node, subSection);
                }
            }
        }

        private void FillParameters(Parameters parameters)
        {
            parametersListView.Items.Clear();
            parametersListView.BeginUpdate();
            foreach (string key in parameters.Keys)
            {
                string value = parameters[key];
                var item = new ListViewItem(key);
                item.Name = key;
                item.SubItems.Add(value);
                parametersListView.Items.Add(item);
            }
            parametersListView.EndUpdate();
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
                sectionNameTextBox.Text = sectionsTreeView.SelectedNode.Name;
            }
            editedParamName = string.Empty;
            paramNameTextBox.Text = string.Empty;
            paramValueTextBox.Text = string.Empty;
        }

        private SectionBase GetSelectedSection()
        {
            TreeNode sectionNode = sectionsTreeView.SelectedNode;
            if (sectionNode != null)
            {
                return (SectionBase)sectionNode.Tag;
            }
            else
            {
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
                var item = new ListViewItem(key);
                item.Name = key;
                item.SubItems.Add(value);
                parametersListView.Items.Add(item);

                section.Parameters[key] = value;

                editedParamName = (string)key.Clone();
                paramNameTextBox.Text = string.Empty;
                paramValueTextBox.Text = string.Empty;

                parametersListView.Focus();
            }
        }

        private void parametersListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ParamToGui();
        }

        private void editParamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string key = paramNameTextBox.Text;
            var section = GetSelectedSection();

            if (!string.IsNullOrEmpty(editedParamName) &&
                !string.IsNullOrEmpty(key))
            {
                string value = paramValueTextBox.Text;
                int index = parametersListView.Items.IndexOfKey(editedParamName);
                if (index >= 0)
                {
                    var item = parametersListView.Items[index];
                    item.Text = key;
                    item.Name = key;
                    item.SubItems[1].Text = value;
                }
                if (!section.Parameters.Keys.Contains(key))
                {
                    section.Parameters.Remove(editedParamName);
                }
                section.Parameters[key] = value;
            }
            editedParamName = (string)key.Clone();
        }

        private void ParamToGui()
        {
            var section = GetSelectedSection();
            if (parametersListView.SelectedIndices.Count > 0)
            {
                var paramName = parametersListView.SelectedItems[0].Name;
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

        private void removeParamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (parametersListView.SelectedIndices.Count > 0)
            {
                var paramName = parametersListView.SelectedItems[0].Name;
                parametersListView.Items.RemoveByKey(paramName);
                var section = GetSelectedSection();
                section.Parameters.Remove(paramName);
            }
        }

        private void addSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newSectionName = sectionNameTextBox.Text;
            if (string.IsNullOrWhiteSpace(newSectionName))
            {
                return;
            }

            TreeNode parentNode = sectionsTreeView.Nodes[0]; // root
            TreeNode selectedNode = sectionsTreeView.SelectedNode;
            if (selectedNode != null)
            {
                parentNode = selectedNode;
            }

            SectionBase parentSection = (SectionBase)parentNode.Tag;
            if (parentSection.Keys.Contains(newSectionName))
            {
                return;
            }
            Sections newSection = new Sections();
            parentSection[newSectionName] = newSection;

            TreeNode newNode = parentNode.Nodes.Add(newSectionName);
            newNode.Name = newSectionName;
            newNode.Tag = newSection;

            sectionsTreeView.Focus();
            sectionsTreeView.SelectedNode = newNode;
        }

        private void editSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newSectionName = sectionNameTextBox.Text;
            if (string.IsNullOrWhiteSpace(newSectionName))
            {
                return;
            }
            TreeNode selectedNode = sectionsTreeView.SelectedNode;
            if ((selectedNode == null) || (selectedNode.Parent == null))
            {
                return;
            }
            var parentSection = (SectionBase)selectedNode.Parent.Tag;
            if (parentSection.Keys.Contains(newSectionName))
            {
                return;
            }
            var section = parentSection[selectedNode.Name];
            parentSection.RemoveSection(selectedNode.Name);
            parentSection[newSectionName] = section;

            selectedNode.Text = newSectionName;
            selectedNode.Name = newSectionName;

            sectionsTreeView.Focus();
        }

        private void removeSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = sectionsTreeView.SelectedNode;
            if (selectedNode != null)
            {
                SectionBase section = (SectionBase)selectedNode.Tag;
                string sectionName = selectedNode.Name;
                if (selectedNode.Parent == null)
                {
                    return; // cannot remove the root node
                }
                SectionBase parentSection = (SectionBase)selectedNode.Parent.Tag;
                parentSection.RemoveSection(sectionName);
                selectedNode.Remove();
            }
        }
    }
}
