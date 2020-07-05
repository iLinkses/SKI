using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;
using System.Data.SQLite;
using System.Drawing.Drawing2D;

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
            Input input = new Input();
            input.mvx = Convert.ToDouble(numericUpDown1.Value);
            input.Tvx = Convert.ToDouble(numericUpDown2.Value) + 273;
            input.G = Convert.ToDouble(numericUpDown3.Value);
            input.Gk = Convert.ToDouble(numericUpDown4.Value);
            input.Ghl = Convert.ToDouble(numericUpDown5.Value);
            input.Thl = Convert.ToDouble(numericUpDown6.Value) + 273;
            input.ch2vx = Convert.ToDouble(numericUpDown7.Value) / 100;

            if (tabControl1.SelectedTab == tabPage1)
            {
                //Перекраска текстбоксов
                textBox1.BackColor = Color.FromArgb(-1);
                textBox9.BackColor = Color.FromArgb(-1);
                textBox10.BackColor = Color.FromArgb(-1);
                textBox11.BackColor = Color.FromArgb(-1);
                textBox12.BackColor = Color.FromArgb(-1);
                textBox13.BackColor = Color.FromArgb(-1);
                textBox14.BackColor = Color.FromArgb(-1);
            }

            //Расчеты
            Сalculation calculation = new Сalculation
            {
                Owner = this
            };
            calculation.Calculation(input.mvx, input.Tvx, input.G, input.Gk, input.Ghl, input.Thl, input.ch2vx);

            SetRowColor();

            if (tabControl1.SelectedTab == tabPage1)
            {
                //Контроль нештатных ситуаций
                if (cbControlES.Checked)
                {
                    ControlES();
                }
            }
        }

        /// <summary>
        /// Проверка на нештатные ситуации
        /// </summary>
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
                    MessageBox.Show("Параметр " + p.Value + " вышел за допустимые нормы.\n\n" + GetES(p.Key), "Нештатная ситуация!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        /// <summary>
        /// Устанавливает цвета для текстбоксов на вкладке "Расчеты"
        /// </summary>
        /// <param name="ID"></param>
        private void SetColor(int ID)
        {
            if (ID == 1)
                textBox1.BackColor = Color.Red;
            if (ID == 2)
                textBox9.BackColor = Color.Red;
            if (ID == 3)
                textBox10.BackColor = Color.Red;
            if (ID == 4)
                textBox11.BackColor = Color.Red;
            if (ID == 5)
                textBox12.BackColor = Color.Red;
            if (ID == 6)
                textBox13.BackColor = Color.Red;
            if (ID == 7)
                textBox14.BackColor = Color.Red;
        }

        private void MathModel_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
            m_dbConn.Open();

            ///Заполняем таблицу на вкладке "Мнемосхема"
            try
            {
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show("Open connection with database");
                }
                string Command = "select TP.ID, " +
                                        "TP.Parameter, " +
                                        "TP.Marking, " +
                                        "TP.Type, " +
                                        "TP.MinVal, " +
                                        "TP.MaxVal, " +
                                        "TP.Value " +
                                 "from Technological_Parameters TP " +
                                 "where(TP.Type = 'Входной' and TP.ID in (14, 15, 16, 19)) " +
                                 "or TP.Type = 'Выходной' " +
                                 "or TP.Type = 'Управляющий' " +
                                 "order by TP.Type = 'Выходной'";
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

            dataGridView1.Rows[4].Cells["Value"].Value = numericUpDown8.Value;
            dataGridView1.Rows[5].Cells["Value"].Value = numericUpDown9.Value;
            dataGridView1.Rows[6].Cells["Value"].Value = numericUpDown10.Value;
            dataGridView1.Rows[7].Cells["Value"].Value = numericUpDown11.Value;
            dataGridView1.Rows[8].Cells["Value"].Value = numericUpDown12.Value;
            dataGridView1.Rows[9].Cells["Value"].Value = numericUpDown13.Value;
            dataGridView1.Rows[10].Cells["Value"].Value = numericUpDown14.Value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e);
            SetRowColor();
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows[11].Index;
            //dataGridView1.Rows[17].Selected = true;
        }

        /// <summary>
        /// Подкрашивает строки таблицы
        /// </summary>
        private void SetRowColor()
        {
            foreach (DataGridViewRow dgvr in dataGridView1.Rows)
            {
                Color RowColor = Color.White;
                //Color SelectionRowColor = SystemColors.GradientActiveCaption;
                //Console.WriteLine(Color.FromArgb(50, SystemColors.GradientActiveCaption));
                if (dataGridView1.Rows[dgvr.Index].Cells["Value"].Value.ToString() != "")
                {
                    if (double.Parse(dataGridView1.Rows[dgvr.Index].Cells["Value"].Value.ToString()) < double.Parse(dataGridView1.Rows[dgvr.Index].Cells["MinVal"].Value.ToString()) || double.Parse(dataGridView1.Rows[dgvr.Index].Cells["Value"].Value.ToString()) > double.Parse(dataGridView1.Rows[dgvr.Index].Cells["MaxVal"].Value.ToString()))
                    {
                        RowColor = Color.Red;
                        //SelectionRowColor = Color.FromArgb(50, SystemColors.GradientActiveCaption);
                    }
                }
                dataGridView1.Rows[dgvr.Index].DefaultCellStyle.BackColor = RowColor;
                dataGridView1.Rows[dgvr.Index].DefaultCellStyle.SelectionForeColor = Color.Black;
                //dataGridView1.Rows[dgvr.Index].DefaultCellStyle.SelectionBackColor = SelectionRowColor;
            }

        }

        #region Связь при изменении значений на вкладках
        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows[4].Index;
            dataGridView1.Rows[4].Selected = true;
            dataGridView1.Rows[4].Cells["Value"].Value = numericUpDown1.Value = numericUpDown8.Value;
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows[4].Index;
            dataGridView1.Rows[5].Selected = true;
            dataGridView1.Rows[5].Cells["Value"].Value = numericUpDown2.Value = numericUpDown9.Value;
        }

        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows[4].Index;
            dataGridView1.Rows[6].Selected = true;
            dataGridView1.Rows[6].Cells["Value"].Value = numericUpDown3.Value = numericUpDown10.Value;
        }

        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows[4].Index;
            dataGridView1.Rows[7].Selected = true;
            dataGridView1.Rows[7].Cells["Value"].Value = numericUpDown4.Value = numericUpDown11.Value;
        }

        private void numericUpDown12_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows[4].Index;
            dataGridView1.Rows[8].Selected = true;
            dataGridView1.Rows[8].Cells["Value"].Value = numericUpDown5.Value = numericUpDown12.Value;
        }

        private void numericUpDown13_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows[4].Index;
            dataGridView1.Rows[9].Selected = true;
            dataGridView1.Rows[9].Cells["Value"].Value = numericUpDown6.Value = numericUpDown13.Value;
        }

        private void numericUpDown14_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows[4].Index;
            dataGridView1.Rows[10].Selected = true;
            dataGridView1.Rows[10].Cells["Value"].Value = numericUpDown7.Value = numericUpDown14.Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows[4].Cells["Value"].Value = numericUpDown8.Value = numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows[5].Cells["Value"].Value = numericUpDown9.Value = numericUpDown2.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows[6].Cells["Value"].Value = numericUpDown10.Value = numericUpDown3.Value;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows[7].Cells["Value"].Value = numericUpDown11.Value = numericUpDown4.Value;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows[8].Cells["Value"].Value = numericUpDown12.Value = numericUpDown5.Value;
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows[9].Cells["Value"].Value = numericUpDown13.Value = numericUpDown6.Value;
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows[10].Cells["Value"].Value = numericUpDown14.Value = numericUpDown7.Value;
        }
        #endregion 
    }
}
