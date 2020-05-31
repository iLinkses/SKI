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


    public partial class MathModel : Form
    {
        readonly String dbFileName = "SKI.db";
        SQLiteConnection m_dbConn;
        SQLiteCommandBuilder sqlCommandBuilder;

        /// <summary>
        /// Структура входных параметров
        /// </summary>
        struct Input
        {
            public double mvx, Tvx, G, Gk, Ghl, Thl, ch2vx;
        }
        /// <summary>
        /// Структура для диапазонов
        /// </summary>
        public struct MinMax
        {
            public double min, max;
        }

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
            //Перекраска текстбоксов
            textBox1.BackColor = Color.FromArgb(-1);
            textBox9.BackColor = Color.FromArgb(-1);
            textBox10.BackColor = Color.FromArgb(-1);
            textBox11.BackColor = Color.FromArgb(-1);
            textBox12.BackColor = Color.FromArgb(-1);
            textBox13.BackColor = Color.FromArgb(-1);
            textBox14.BackColor = Color.FromArgb(-1);

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

            //Контроль нештатных ситуаций
            if (cbControlES.Checked)
            {
                ControlES();
            }
        }

        private void ControlES()
        {
            Dictionary<int, double> Output = new Dictionary<int, double>(7);
            Output.Add(1, double.Parse(textBox1.Text));
            Output.Add(2, double.Parse(textBox9.Text));
            Output.Add(3, double.Parse(textBox10.Text));
            Output.Add(4, double.Parse(textBox11.Text));
            Output.Add(5, double.Parse(textBox12.Text));
            Output.Add(6, double.Parse(textBox13.Text));
            Output.Add(7, double.Parse(textBox14.Text));
            int check = 0;
            //bool check = false;

            List<MinMax> minmax = new List<MinMax>();
            foreach (var p in Output)
            {
                minmax = GetMin_Max(p.Key);
                if (p.Value < minmax[0].min || p.Value > minmax[0].max)
                {
                    SetColor(p.Key);
                    MessageBox.Show("Параметр "+ p.Value + " вышел за допустимые нормы.\n\n" +GetES(p.Key),"Нештатная ситуация!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    check++;
                }
            }
            if (check == 7)
            {
                MessageBox.Show("Все параметры в норме!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Метод возвращает минимальное и максимальное значение по ID параметра
        /// </summary>
        /// <param name="ID">ID параметра</param>
        public List<MinMax> GetMin_Max(int ID)
        {
            List<MinMax> minmax = new List<MinMax>();

            m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
            m_dbConn.Open();

            try
            {
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show("Open connection with database");
                }
                string Command = "select TP.MinVal, TP.MaxVal " +
                                 "from Technological_Parameters TP " +
                                 "where TP.ID = " + ID;
                SQLiteDataAdapter sqlAdapter = new SQLiteDataAdapter(Command, m_dbConn);
                sqlCommandBuilder = new SQLiteCommandBuilder(sqlAdapter);

                DataTable dTable = new DataTable();
                sqlAdapter.Fill(dTable);
                minmax.Add(new MinMax() { min = double.Parse(dTable.Rows[0][0].ToString()), max = double.Parse(dTable.Rows[0][1].ToString()) });
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            m_dbConn.Close();

            return minmax;
        }

        /// <summary>
        /// Метод получает связку: ситуация->причина->рекомендация
        /// </summary>
        /// <param name="ID">ID параметра</param>
        private string GetES(int ID)
        {
            string ES = "";
            m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
            m_dbConn.Open();

            try
            {
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show("Open connection with database");
                }
                string Command = "select TPES.Situation, TPRe.Reason, TPR.Recommendation " +
                                 "from TP_Recommendations TPR " +
                                 "inner join TP_Reasons TPRe on TPRe.ID = TPR.ID_Reasons " +
                                 "inner join TP_ES TPES on TPES.ID = TPRe.ID_TP_ES " +
                                 "where TPES.ID_TP = " + ID;
                SQLiteDataAdapter sqlAdapter = new SQLiteDataAdapter(Command, m_dbConn);
                sqlCommandBuilder = new SQLiteCommandBuilder(sqlAdapter);
                DataTable dTable = new DataTable();
                sqlAdapter.Fill(dTable);
                if (dTable.Rows.Count != 0)
                {
                    int index = 1;
                    ES = "Ситуация: " + dTable.Rows[0]["Situation"].ToString() +
                         "\nПричины:\n";
                    foreach (DataRow dr in dTable.Rows)
                    {
                        ES += index.ToString() + ". " + dr["Reason"].ToString() + "\n";
                        ES += "Рекомендация: " + dr["Recommendation"].ToString() + "\n";
                        index++;
                    }
                }
                else
                {
                    ES = "Данный параметр вышел за допустимые нормы, но для него нет описания ¯\\_(ツ)_/¯";
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            m_dbConn.Close();

            return ES;
        }

        private void SetColor(int ID)
        {
            if (ID == 1)
                textBox1.BackColor = Color.Red;
            if (ID == 2)
                textBox9.BackColor = Color.Red;
            if (ID == 3)
                textBox10.BackColor = Color.Red;
            if(ID == 4)
                textBox11.BackColor = Color.Red;
            if(ID==5)
                textBox12.BackColor = Color.Red;
            if(ID==6)
                textBox13.BackColor = Color.Red;
            if(ID==7)
                textBox14.BackColor = Color.Red;
        }

        private void MathModel_Load(object sender, EventArgs e)
        {
            m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
            m_dbConn.Open();

            try
            {
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show("Open connection with database");
                }
                string Command = "select TP.Parameter, " +
                                        "TP.MinVal, " +
                                        "TP.MaxVal, " +
                                        "TP.Value " +
                                 "from Technological_Parameters TP " +
                                 "where TP.Type = 'Входной'";
                SQLiteDataAdapter sqlAdapter = new SQLiteDataAdapter(Command, m_dbConn);
                sqlCommandBuilder = new SQLiteCommandBuilder(sqlAdapter);

                DataTable dTable = new DataTable();
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
    }
}
