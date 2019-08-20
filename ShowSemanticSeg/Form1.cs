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
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace ShowSemanticSeg
{
    using tasktype = Tuple<UserControl1, string>;
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
                        var cont = new UserControl1(name, this);
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
            this.picture_update();
        }
        public void DeccIndex(Control obj)
        {
            var index = this.panel1.Controls.IndexOf(obj);
            if (index == this.panel1.Controls.Count - 1) return;
            this.panel1.Controls.SetChildIndex(obj, index + 1);
            this.picture_update();
        }

        public async void picture_update()
        {
            this.panel1.Invalidate();

            if (this.label2.Text == "") return;
            var file_name = this.label2.Text;

            this.pictureBox1.SuspendLayout();
            var tasks = new List<Task<Bitmap>>();
            foreach (UserControl1 element in this.panel1.Controls)
            {
                var args = new tasktype(element, file_name);
                tasks.Add(Task<Bitmap>.Run(() => this.image_task(args)));
            }
            var ret = await Task<Bitmap>.WhenAll( tasks);

            var temp = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            foreach (var element in ret)
            {
                if (element == null) continue;

                element.MakeTransparent(Color.Black);

                System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix();
                //ColorMatrixの行列の値を変更して、アルファ値が0.5に変更されるようにする
                cm.Matrix00 = 1;
                cm.Matrix11 = 1;
                cm.Matrix22 = 1;
                cm.Matrix33 = 0.5F;
                cm.Matrix44 = 1;

                //ImageAttributesオブジェクトの作成
                System.Drawing.Imaging.ImageAttributes ia =
                    new System.Drawing.Imaging.ImageAttributes();
                //ColorMatrixを設定する
                ia.SetColorMatrix(cm);
                Graphics.FromImage(temp).DrawImage(
                    element,
                    new Rectangle(0,0,element.Width,element.Height),
                    0,0,this.pictureBox1.Width,this.pictureBox1.Height,
                    GraphicsUnit.Pixel,ia);
            }
            this.pictureBox1.Image = temp;
            this.pictureBox1.ResumeLayout();
        }
        
        private Bitmap image_task(tasktype args) 
        {
            var cont = args.Item1;
            var file_name = args.Item2;
            if (cont.Enable == false) return null;
            Mat mat;
            using (var archive = ZipFile.Open(file_name, ZipArchiveMode.Read))
            using (var entry = archive.GetEntry(cont.FileName).Open())
            {
                mat = new Bitmap(entry).ToMat();
            }
            var hsv_mat = mat.CvtColor(ColorConversionCodes.BGR2HSV);
            for (int i = 0; i < hsv_mat.Height; i++)
            {
                for (int j = 0; j < hsv_mat.Width; j++)
                {
                    Vec3b pix = hsv_mat.At<Vec3b>(i, j);
                    pix[0] = (byte)(cont.Color.GetHue() / 2);
                    pix[1] = (byte)(cont.Color.GetSaturation() * 255);
                    pix[2] = (byte)(cont.Color.GetBrightness() * pix[2]);
                    hsv_mat.Set<Vec3b>(i, j, pix);
                }
            }
            mat = hsv_mat.CvtColor(ColorConversionCodes.HSV2BGR);
            return mat.ToBitmap();
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            int count = 0;
            foreach (UserControl1 element in this.panel1.Controls)
            {
                element.Top = element.Height * count++;
            }
        }
    }
}
