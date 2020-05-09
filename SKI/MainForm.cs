using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!Program._IsAdmUser)
            {
                математическаяМодельToolStripMenuItem.Checked = true;
                математическаяМодельToolStripMenuItem.Enabled = false;
                определениеНештатныхСитуацийToolStripMenuItem.Checked = false;
                определениеНештатныхСитуацийToolStripMenuItem.Enabled = true;
                MathModel MathModel = new MathModel();
                MathModel.MdiParent = this;
                this.SetClientSizeCore(MathModel.Width + 4, MathModel.Height + 20 + 26 + 4);
                toolStripStatusLabel1.Text = "Математическая модель";
                //this.Size = SizeFromClientSize(MathModel.Size);
                MathModel.Show();
            }
            else if (Program._IsAdmUser)
            {
                модульToolStripMenuItem.Visible = false;
                AdmnForm admnForm = new AdmnForm();
                admnForm.MdiParent = this;
                this.SetClientSizeCore(admnForm.Width + 4, admnForm.Height + 20 + 26 + 4);
                toolStripStatusLabel1.Text = "Модуль администратора";
                //this.Size = SizeFromClientSize(MathModel.Size);
                admnForm.Show();
            }
        }

        private void математическаяМодельToolStripMenuItem_Click(object sender, EventArgs e)
        {
            математическаяМодельToolStripMenuItem.Checked = true;
            математическаяМодельToolStripMenuItem.Enabled = false;
            определениеНештатныхСитуацийToolStripMenuItem.Checked = false;
            определениеНештатныхСитуацийToolStripMenuItem.Enabled = true;
            ActiveMdiChild.Close();
            MathModel MathModel = new MathModel();
            MathModel.MdiParent = this;
            this.SetClientSizeCore(MathModel.Width + 4, MathModel.Height + 20 + 26 + 4);
            toolStripStatusLabel1.Text = "Математическая модель";
            //this.Size = SizeFromClientSize(MathModel.Size);
            MathModel.Show();
        }

        private void определениеНештатныхСитуацийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            математическаяМодельToolStripMenuItem.Checked = false;
            математическаяМодельToolStripMenuItem.Enabled = true;
            определениеНештатныхСитуацийToolStripMenuItem.Checked = true;
            определениеНештатныхСитуацийToolStripMenuItem.Enabled = false;
            ActiveMdiChild.Close();
            ESForm eSForm = new ESForm();
            eSForm.MdiParent = this;
            this.SetClientSizeCore(eSForm.Width + 4, eSForm.Height + 20 + 26 + 4);
            toolStripStatusLabel1.Text = "Определение нештатных ситуаций";
            eSForm.Show();
        }  
    }
}
