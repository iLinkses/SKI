using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using System.Data.SQLite;

namespace SKI
{
    struct Input
    {
        public double mvx, Tvx, G, Gk, Ghl, Thl, ch2vx;
    }
    struct Output
    {
       public string muni, pl, pm, m, T, ch2, d;
    }
    //struct RezultQuery
    //{
    //    public 
    //}

    public partial class MathModel : Form
    {
        public MathModel()
        {
            InitializeComponent();
            // Включим показ всплывающих подсказок при наведении курсора на график
            zedGraphControl1.IsShowPointValues = true;
            // Будем обрабатывать событие PointValueEvent, чтобы изменить формат представления координат
            zedGraphControl1.PointValueEvent += new ZedGraphControl.PointValueHandler(zedGraph_PointValueEvent);

            zedGraphControl2.IsShowPointValues = true;
            zedGraphControl2.PointValueEvent += new ZedGraphControl.PointValueHandler(zedGraph_PointValueEvent);

            zedGraphControl3.IsShowPointValues = true;
            zedGraphControl3.PointValueEvent += new ZedGraphControl.PointValueHandler(zedGraph_PointValueEvent);

            zedGraphControl4.IsShowPointValues = true;
            zedGraphControl4.PointValueEvent += new ZedGraphControl.PointValueHandler(zedGraph_PointValueEvent);

            zedGraphControl5.IsShowPointValues = true;
            zedGraphControl5.PointValueEvent += new ZedGraphControl.PointValueHandler(zedGraph_PointValueEvent);

            zedGraphControl6.IsShowPointValues = true;
            zedGraphControl6.PointValueEvent += new ZedGraphControl.PointValueHandler(zedGraph_PointValueEvent);

            zedGraphControl7.IsShowPointValues = true;
            zedGraphControl7.PointValueEvent += new ZedGraphControl.PointValueHandler(zedGraph_PointValueEvent);
        }

        string zedGraph_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            // Получим точку, около которой находимся
            PointPair point = curve[iPt];

            // Сформируем строку
            string result = string.Format("Значение: {0:N3}", point.Y);

            return result;
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            Input input = new Input();
            input.mvx = Convert.ToDouble(numericUpDown1.Value);
            input.Tvx = Convert.ToDouble(numericUpDown2.Value) + 273;
            input.G = Convert.ToDouble(numericUpDown3.Value);
            input.Gk = Convert.ToDouble(numericUpDown4.Value);
            input.Ghl = Convert.ToDouble(numericUpDown5.Value);
            input.Thl = Convert.ToDouble(numericUpDown6.Value) + 273;
            input.ch2vx = Convert.ToDouble(numericUpDown7.Value) / 100;

            //Очистка боксов для выходных параметров
            textBox1.Clear();
            textBox9.Clear();
            textBox10.Clear();
            textBox11.Clear();
            textBox12.Clear();
            textBox13.Clear();
            textBox14.Clear();

            //Расчеты
            Сalculation calculation = new Сalculation
            {
                Owner = this
            };
            calculation.Calculation(input.mvx, input.Tvx, input.G, input.Gk, input.Ghl, input.Thl, input.ch2vx);

            if (cbControlES.Checked)
            {
                Output Commands = new Output();
                Commands.muni = "SELECT 'muni',(SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'muni' AND MinVal <=" + textBox1.Text.Replace(",", ".") + " AND MaxVal >=" + textBox1.Text.Replace(",", ".") + "))";
                Commands.pl = "SELECT 'pl',(SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'pl' AND MinVal <=" + textBox9.Text.Replace(",", ".") + " AND MaxVal >=" + textBox9.Text.Replace(",", ".") + "))";
                Commands.pm = "SELECT 'pm',(SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'pm' AND MinVal <=" + textBox10.Text.Replace(",", ".") + " AND MaxVal >=" + textBox10.Text.Replace(",", ".") + "))";
                Commands.m = "SELECT 'm',(SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'm' AND MinVal <=" + textBox11.Text.Replace(",", ".") + " AND MaxVal >=" + textBox11.Text.Replace(",", ".") + "))";
                Commands.T = "SELECT 'T',(SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'T' AND MinVal <=" + textBox12.Text.Replace(",", ".") + " AND MaxVal >=" + textBox12.Text.Replace(",", ".") + "))";
                Commands.ch2 = "SELECT 'ch2',(SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'ch2' AND MinVal <=" + textBox13.Text.Replace(",", ".") + " AND MaxVal >=" + textBox13.Text.Replace(",", ".") + "))";
                Commands.d = "SELECT 'd',(SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'd' AND MinVal <=" + textBox14.Text.Replace(",", ".") + " AND MaxVal >=" + textBox14.Text.Replace(",", ".") + "))";
                ControlES(Commands);
            }
        }

        struct SQL
        {
            public SQLiteDataAdapter AMuni, APl, Apm, Am, AT, Ach2, Ad;
            public SQLiteCommandBuilder CBMuni, CBPl, CBpm, CBm, CBT, CBch2, CBd;
        }
        /// <summary>
        /// Метод проверяющий диапазон выходных параметров
        /// </summary>
        /// <param name="Commands">Структура выходных параметров</param>
        private void ControlES(Output Commands)
        {

            String dbFileName = "SKI.db";
            SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
            m_dbConn.Open();
            
            try
            {
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show("Open connection with database");
                    return;
                }
                SQL sql = new SQL();
                sql.AMuni = new SQLiteDataAdapter(Commands.muni, m_dbConn);
                sql.CBMuni = new SQLiteCommandBuilder(sql.AMuni);
                sql.APl = new SQLiteDataAdapter(Commands.pl, m_dbConn);
                sql.CBPl = new SQLiteCommandBuilder(sql.APl);
                sql.Apm = new SQLiteDataAdapter(Commands.pm, m_dbConn);
                sql.CBpm = new SQLiteCommandBuilder(sql.Apm);
                sql.Am = new SQLiteDataAdapter(Commands.m, m_dbConn);
                sql.CBm = new SQLiteCommandBuilder(sql.Am);
                sql.AT = new SQLiteDataAdapter(Commands.T, m_dbConn);
                sql.CBT = new SQLiteCommandBuilder(sql.AT);
                sql.Ach2 = new SQLiteDataAdapter(Commands.ch2, m_dbConn);
                sql.CBch2 = new SQLiteCommandBuilder(sql.Ach2);
                sql.Ad = new SQLiteDataAdapter(Commands.d, m_dbConn);
                sql.CBd = new SQLiteCommandBuilder(sql.Ad);

                DataTable dTable = new DataTable();
                dTable.Rows.Add(sql.AMuni.Fill(dTable));
                dTable.Rows.Add(sql.APl.Fill(dTable));
                dTable.Rows.Add(sql.Apm.Fill(dTable));
                //sql.AMuni.Fill(dTable);
                //sql.APl.Fill(dTable);
                //sql.Apm.Fill(dTable);
                foreach (DataRow r in dTable.Rows)
                {
                    foreach (var cell in r.ItemArray)
                        Console.Write(cell);
                }
                var testMuni = dTable.Rows[2][0].ToString();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
