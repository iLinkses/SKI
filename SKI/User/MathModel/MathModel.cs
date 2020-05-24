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

            //Контроль нештатных ситуаций
            if (cbControlES.Checked)
            {
                string Command = "SELECT TP.Parameter, (SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'muni' AND MinVal <=" + textBox1.Text.Replace(",", ".") + " AND MaxVal >=" + textBox1.Text.Replace(",", ".") + ")) as Value " +
                                 "from Technological_Parameters TP " +
                                 "where TP.Parameter in('muni') " +
                                 "union " +
                                 "SELECT TP.Parameter, (SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'pl' AND MinVal <= " + textBox9.Text.Replace(",", ".") + " AND MaxVal >=" + textBox9.Text.Replace(",", ".") + ")) as Value " +
                                 "from Technological_Parameters TP " +
                                 "where TP.Parameter in('pl') " +
                                 "union " +
                                 "SELECT TP.Parameter, (SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'pm' AND MinVal <= " + textBox10.Text.Replace(",", ".") + " AND MaxVal >=" + textBox10.Text.Replace(",", ".") + ")) as Value " +
                                 "from Technological_Parameters TP " +
                                 "where TP.Parameter in('pm') " +
                                 "union " +
                                 "SELECT TP.Parameter, (SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'm' AND MinVal <= " + textBox11.Text.Replace(",", ".") + " AND MaxVal >=" + textBox11.Text.Replace(",", ".") + ")) as Value " +
                                 "from Technological_Parameters TP " +
                                 "where TP.Parameter in('m') " +
                                 "union " +
                                 "SELECT TP.Parameter, (SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'T' AND MinVal <= " + textBox12.Text.Replace(",", ".") + " AND MaxVal >=" + textBox12.Text.Replace(",", ".") + ")) as Value " +
                                 "from Technological_Parameters TP " +
                                 "where TP.Parameter in('T') " +
                                 "union " +
                                 "SELECT TP.Parameter, (SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'ch2' AND MinVal <= " + textBox13.Text.Replace(",", ".") + " AND MaxVal >=" + textBox13.Text.Replace(",", ".") + ")) as Value " +
                                 "from Technological_Parameters TP " +
                                 "where TP.Parameter in('ch2') " +
                                 "union " +
                                 "SELECT TP.Parameter, (SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'd' AND MinVal <= " + textBox14.Text.Replace(",", ".") + " AND MaxVal >=" + textBox14.Text.Replace(",", ".") + ")) as Value " +
                                 "from Technological_Parameters TP " +
                                 "where TP.Parameter in('d')";
                //"SELECT (SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'muni' AND MinVal <=" + textBox1.Text.Replace(",", ".") + " AND MaxVal >=" + textBox1.Text.Replace(",", ".") + ")) as muni, " +
                //                        "(SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'pl' AND MinVal <= " + textBox9.Text.Replace(",", ".") + " AND MaxVal >=" + textBox9.Text.Replace(",", ".") + ")) as pl, " +
                //                        "(SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'pm' AND MinVal <= " + textBox10.Text.Replace(",", ".") + " AND MaxVal >=" + textBox10.Text.Replace(",", ".") + ")) as pm, " +
                //                        "(SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'm' AND MinVal <= " + textBox11.Text.Replace(",", ".") + " AND MaxVal >=" + textBox11.Text.Replace(",", ".") + ")) as m, " +
                //                        "(SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'T' AND MinVal <= " + textBox12.Text.Replace(",", ".") + " AND MaxVal >=" + textBox12.Text.Replace(",", ".") + ")) as T, " +
                //                        "(SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'ch2' AND MinVal <= " + textBox13.Text.Replace(",", ".") + " AND MaxVal >=" + textBox13.Text.Replace(",", ".") + ")) as ch2, " +
                //                        "(SELECT EXISTS(SELECT * FROM Technological_Parameters TP WHERE TP.Parameter = 'd' AND MinVal <= " + textBox14.Text.Replace(",", ".") + " AND MaxVal >=" + textBox14.Text.Replace(",", ".") + ")) as d";
                ControlES(Command);
            }
        }

        /// <summary>
        /// Метод проверяющий диапазон выходных параметров
        /// </summary>
        /// <param name="Commands">Структура выходных параметров</param>
        private void ControlES(string Command)
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
                SQLiteDataAdapter sqlAdapter = new SQLiteDataAdapter(Command, m_dbConn);
                SQLiteCommandBuilder sqlCommandBuilder = new SQLiteCommandBuilder(sqlAdapter);

                DataTable dTable = new DataTable();
                sqlAdapter.Fill(dTable);
                //foreach (DataColumn column in dTable.Columns)
                //    Console.Write("\t{0}", column.ColumnName);
                //Console.WriteLine();
                //// перебор всех строк таблицы
                //foreach (DataRow row in dTable.Rows)
                //{
                //    // получаем все ячейки строки
                //    var cells = row.ItemArray;
                //    foreach (object cell in cells)
                //        Console.Write("\t{0}", cell);
                //    Console.WriteLine();
                //}

                var checkES = from r in dTable.AsEnumerable()
                              where r.Field<long>("Value") == 0
                              select r.Field<string>("Parameter");
                foreach (var t in checkES)
                {
                    Console.WriteLine(t);
                }
                var testMuni = dTable.Rows[0][0].ToString();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void searchES(DataTable dt)
        {

        }
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
