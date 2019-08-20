using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShowSemanticSeg
{
    public partial class UserControl1 : UserControl
    {
        Form1 parent;

        public string FileName { get; }
        public bool Enable { get { return this.checkBox1.Checked; }set { this.checkBox1.Checked = value; } }

        public Color Color { get { return this.colorDialog1.Color; } }

        public UserControl1(string name,Form1 parent)
        {
            InitializeComponent();
            this.FileName = name;
            this.checkBox1.Text = this.FileName;
            this.parent = parent;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.parent.IncIndex(this);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.parent.DeccIndex(this);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            var dialog = this.colorDialog1;
            dialog.ShowHelp = true;
            if ( dialog.ShowDialog(this.parent) == DialogResult.OK)
            {
                this.button3.BackColor = dialog.Color;
            }
            this.parent.picture_update();
        }

        private void UserControl1_DoubleClick(object sender, EventArgs e)
        {
            this.parent.OnlySelect(this);
        }

        private void CheckBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            this.parent.picture_update();
        }
    }
}
