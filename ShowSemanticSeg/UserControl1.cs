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
        string file_name;

        public string FileName { get; }
        public bool Enable { get { return this.checkBox1.Checked; } }
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

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.parent.Refresh();
        }
    }
}
