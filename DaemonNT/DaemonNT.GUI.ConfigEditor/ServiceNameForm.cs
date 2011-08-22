using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DaemonNT.GUI.ConfigEditor
{
    public partial class UniqueNameForm : Form
    {
        public string EditedName { get; set; }
        public IList<string> ExistingNames { get; set; }

        public UniqueNameForm(string formTitle, string message)
        {
            InitializeComponent();
            Text = formTitle;
            messageLabel.Text = message;
            ExistingNames = new List<string>();
        }

        private void serviceNameForm_Load(object sender, EventArgs e)
        {
            serviceNameTextBox.Text = EditedName;
            serviceNameTextBox.Focus();
            EnableOkButtonOnGoodServiceName();
        }

        private void serviceNameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                EditedName = serviceNameTextBox.Text;
            }
        }

        private void serviceNameTextBox_TextChanged(object sender, EventArgs e)
        {
            EnableOkButtonOnGoodServiceName();
        }

        private void EnableOkButtonOnGoodServiceName()
        {
            okButton.Enabled =
                !string.IsNullOrWhiteSpace(serviceNameTextBox.Text) &&
                !ExistingNames.Contains(serviceNameTextBox.Text);
        }
    }
}
