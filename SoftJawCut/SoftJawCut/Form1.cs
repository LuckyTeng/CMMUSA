using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SoftJawCut
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cbRough.SelectedIndex = 0;
            cbFinish.SelectedIndex = 0;
        }

        private void cbSameSize_CheckedChanged(object sender, EventArgs e)
        {
            nudDown.Enabled = !cbSameSize.Checked;
            if ( cbSameSize.Checked )
            {
                nudDown.Value = nudUp.Value;
            }
        }

        private void nudUp_ValueChanged(object sender, EventArgs e)
        {
            if ( cbSameSize.Checked )
            {
                nudDown.Value = nudUp.Value;
            }
        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            if ( saveDiag.ShowDialog() == DialogResult.OK )
            {
                txtOutFile.Text = saveDiag.FileName;
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            HassGCodeProvider provider = new HassGCodeProvider();
            PathGenerator pathGenerator = new PathGenerator();

            var pa = PathArgumentBuilder.Build(Convert.ToInt32(cbRough.Text),
                                               Convert.ToInt32(cbFinish.Text),
                                               Convert.ToInt32(nudDeep.Value),
                                               Convert.ToInt32(nudUp.Value),
                                               Convert.ToInt32(nudDown.Value),
                                               Convert.ToInt32(nudCutLen.Value));

            string output = provider.Parse(pathGenerator.Generator(pa));

            File.WriteAllText(txtOutFile.Text, output);
        }
    }
}
