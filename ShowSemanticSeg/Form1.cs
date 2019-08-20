using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShowSemanticSeg
{
    public partial class Form1 : Form
    {
        private List<string> file_list;
        public Form1()
        {
            InitializeComponent();
            file_list = new List<string>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void MenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try // read a input zipfile
            {
                string file_name;

                using (var dialog = new System.Windows.Forms.OpenFileDialog()) { 
                    if (dialog.ShowDialog() != DialogResult.OK) return;
                    file_name = dialog.FileName;
                    this.label2.Text = file_name;
                }

                using (var zipFile = ZipFile.Open(file_name, ZipArchiveMode.Read))
                {
                    this.panel1.Controls.Clear();
                    foreach (ZipArchiveEntry entry in zipFile.Entries)
                    {
                        var name = entry.FullName;
                        file_list.Add(name);
                        var cont = new UserControl1(name,this);
                        this.panel1.Controls.Add(cont);
                    }
                }
            }
            catch (NotSupportedException exce)
            {
                return;
            }
        }

        public void IncIndex(Control obj)
        {
            var index = this.panel1.Controls.IndexOf(obj);
            if (index < 1) return;
            this.panel1.Controls.SetChildIndex(obj, index - 1);
            this.panel1.Invalidate();
            this.pictureBox1.Invalidate();
        }
        public void DeccIndex(Control obj)
        {
            var index = this.panel1.Controls.IndexOf(obj);
            if (index == this.panel1.Controls.Count -1) return;
            this.panel1.Controls.SetChildIndex(obj, index + 1);
            this.panel1.Invalidate();
            this.pictureBox1.Invalidate();
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            int count = 0 ;
            foreach(UserControl1 element in this.panel1.Controls)
            {
                element.Top = element.Height * count++;
                element.Invalidate();
            }
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (this.label2.Text == "") return;
            var file_name = this.label2.Text;
            foreach (UserControl1 element in this.panel1.Controls)
            {
                if (element.Enable == false) continue;

                using (var zipFile = ZipFile.Open(file_name, ZipArchiveMode.Read))
                {
                    using (var entry = zipFile.GetEntry(element.FileName).Open())
                    {
                        this.pictureBox1.Image = Image.FromStream(entry);
                    }
                }
            }
        }
    }
}
