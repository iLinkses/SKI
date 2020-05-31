
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
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


        private void Main_Load(object sender, EventArgs e)
        {
            FillTree();
            GetInfluenceOfImpurities();
        }

        private void GetInfluenceOfImpurities()
        {
            DataTable dTable = new DataTable();
            String dbFileName = "SKI.db";
            SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
            m_dbConn.Open();

            try
            {
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show("Open connection with database");
                }
                string Command = "select IOI.Impurity, IOI.DecreaseOfReactionRate, IOI.ReducingTheMolecularWeight " +
                                 "from Influence_Of_Impurities IOI";
                SQLiteDataAdapter sqlAdapter = new SQLiteDataAdapter(Command, m_dbConn);
                SQLiteCommandBuilder sqlCommandBuilder = new SQLiteCommandBuilder(sqlAdapter);


                sqlAdapter.Fill(dTable);
                BindingSource bindingSource = new BindingSource();
                bindingSource.DataSource = dTable;
                dataGridView1.DataSource = bindingSource;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            m_dbConn.Close();
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
    }
}
