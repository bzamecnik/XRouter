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
    public partial class ServiceNameForm : Form
    {
        public string ServiceName { get; set; }
        public IList<string> ExistingNames { get; set; }

        public ServiceNameForm()
        {
            InitializeComponent();
            ExistingNames = new List<string>();
        }

        private void serviceNameForm_Load(object sender, EventArgs e)
        {
            serviceNameTextBox.Text = ServiceName;
            serviceNameTextBox.Focus();
        }

        private void serviceNameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                ServiceName = serviceNameTextBox.Text;
            }
        }

        private void serviceNameTextBox_TextChanged(object sender, EventArgs e)
        {
            okButton.Enabled = !ExistingNames.Contains(serviceNameTextBox.Text);
        }
    }
}
