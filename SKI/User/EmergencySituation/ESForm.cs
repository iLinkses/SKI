
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SQLiteDBConnection;

namespace SKI
{
    public partial class ESForm : Form
    {
        SQLiteDB db = new SQLiteDB();
        
        public ESForm()
        {
            //---
            InitializeComponent();
            //---
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.DrawMode = DrawMode.OwnerDrawFixed;
            this.comboBox1.DrawItem += new DrawItemEventHandler(comboBox1_DrawItem);
            this.comboBox1.DropDownClosed += new EventHandler(comboBox1_DropDownClosed);
            this.comboBox1.MouseLeave += new EventHandler(comboBox1_Leave);

            //Заполнение comboBox1 (Нештатные ситуации)
            var ES = db.GetES();
            foreach (var EmergencySituation in ES)
            {
                comboBox1.Items.Add(EmergencySituation.EmergencySituation.ToString());
            }
        }

        //Заполнение treeView из базы
        void FillTree()
        {
            //создание корневого узла
            TreeNode rootNode = new TreeNode()
            {
                Name = "ES",
                Text = "Нештатные ситуации",
            };
            treeView.Nodes.Add(rootNode);

            //добавление нештатных ситуаций
            var ES = db.GetES();
            foreach (var EmergencySituation in ES)
            {
                TreeNode ESituation = new TreeNode()
                {

                    Name = EmergencySituation.ID_ES.ToString(),//ES.Count.ToString(),
                    Text = EmergencySituation.EmergencySituation.ToString(),
                };
                rootNode.Nodes.Add(ESituation);
            }

            //добавление причин нештатных ситуаций
            var CES = db.GetCES();
            foreach (var CausesOfES in CES)
            {
                TreeNode COfES = new TreeNode()
                {
                    Name = CausesOfES.ID_CES.ToString(),//CES.Count.ToString(),
                    Text = CausesOfES.CausesOfES.ToString(),
                };
                int idES = Convert.ToInt32(CausesOfES.ParentID);
                rootNode.Nodes[idES].Nodes.Add(COfES);
            }

            //Добавление дополнительных параметров у возникших причин
            var TCES = db.GetTCES();
            foreach (var TipOfCES in TCES)
            {
                foreach (var IDES in CES)
                {
                    if (Convert.ToInt32(IDES.Tip) == 1)
                    {
                        if (Convert.ToInt32(IDES.ID_CES) == Convert.ToInt32(TipOfCES.ID_CES))
                        {
                            TreeNode TOfCES = new TreeNode()
                            {
                                Name = TipOfCES.ID_TCES.ToString(),//TCES.Count.ToString(),
                                Text = TipOfCES.TipOfCES.ToString(),
                            };
                            int idES = Convert.ToInt32(IDES.ParentID);
                            int idCES = Convert.ToInt32(TipOfCES.ParentID);
                            rootNode.Nodes[idES].Nodes[idCES].Nodes.Add(TOfCES);
                        }
                    }
                }
            }
        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) { return; }
            string text = comboBox1.GetItemText(comboBox1.Items[e.Index]);
            e.DrawBackground();
            using (SolidBrush br = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(text, e.Font, br, e.Bounds);
            }

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected && comboBox1.DroppedDown)
            {
                if (TextRenderer.MeasureText(text, comboBox1.Font).Width > comboBox1.Width)
                {
                    toolTip1.Show(text, comboBox1, e.Bounds.Right, e.Bounds.Bottom);
                }
                else
                {
                    toolTip1.Hide(comboBox1);
                }
            }
            e.DrawFocusRectangle();
        }

        private void comboBox1_MouseHover(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                if (!comboBox1.DroppedDown && TextRenderer.MeasureText(comboBox1.SelectedItem.ToString(), comboBox1.Font).Width > comboBox1.Width)
                {
                    toolTip1.Show(comboBox1.SelectedItem.ToString(), comboBox1, comboBox1.Location.X, comboBox1.Location.Y);
                }
            }
        }

        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            toolTip1.Hide(comboBox1);
        }

        private void comboBox1_Leave(object sender, EventArgs e)
        {
            toolTip1.Hide(comboBox1);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            FillTree();
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            textBox1.Clear();
            int level = e.Node.Level;
            int index = e.Node.Index;

            if (level == 2)
            {
                int idCES = Convert.ToInt32(e.Node.Name);
                int ChildNodesCount = e.Node.TreeView.SelectedNode.Nodes.Count;
                if (ChildNodesCount == 0)
                {
                    SES2Lvl(idCES, e.Node.Text);
                }
                
            }
            if (level == 3)
            {
                int idTCES = Convert.ToInt32(e.Node.Name);
                SES3Lvl(idTCES, e.Node.Text);
            }
            //textBox1.Text = treeView.SelectedNode.Text + " " + index + level;
        }

        //Функция вывода решения ситуаций
        void SES2Lvl(int CESID, string CES)
        {
            int ses1 = Convert.ToInt32(CESID);
            var SES = db.GetSES().SingleOrDefault(s => s.ID_CES == ses1);
            if (SES == null) return;
            //не переносит текст на другую строку
            textBox1.Text = CES + Environment.NewLine + Environment.NewLine + SES.SolutionOfES;
        }
        //Функция вывода решения ситуаций
        void SES3Lvl(int TCESID, string TCES)
        {
            int ses1 = Convert.ToInt32(TCESID);
            var SES = db.GetSES().SingleOrDefault(s => s.ID_TCES == ses1);
            if (SES == null) return;
            //не переносит текст на другую строку
            textBox1.Text = TCES + Environment.NewLine + Environment.NewLine + SES.SolutionOfES;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            textBox9.Clear();
            double M, Pm, t1, t2, fi, Ge, Pl, W;
            M = Convert.ToDouble( textBoxM.Text);
            Pm = Convert.ToDouble(textBoxPm.Text);//Что то не то при измении
            t1 = Convert.ToDouble(textBoxt1.Text);
            t2 = Convert.ToDouble(textBoxt2.Text);
            fi = Convert.ToDouble(textBoxfi.Text);
            Ge = Convert.ToDouble(textBoxGe.Text);
            Pl = Convert.ToDouble(textBoxPl.Text);
            W = Convert.ToDouble(textBoxW.Text);
            AnalysisByParameters ABP = new AnalysisByParameters();
           textBox9.Text = ABP.Analysis(M, Pm, t1, t2, fi, Ge, Pl, W); 
        }
    }
}
