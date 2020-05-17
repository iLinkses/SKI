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
       public double muni, pl, pm, m, T, ch2, d;
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
            Input input;
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
                Output output;
                output.muni = double.Parse(textBox1.Text);
                output.pl = double.Parse(textBox9.Text);
                output.pm = double.Parse(textBox10.Text);
                output.m = double.Parse(textBox11.Text);
                output.T = double.Parse(textBox12.Text);
                output.ch2 = double.Parse(textBox13.Text);
                output.d = double.Parse(textBox14.Text);
                ControlES(output);
            }
        }

        /// <summary>
        /// Метод проверяющий диапазон выходных параметров
        /// </summary>
        /// <param name="output">Структура выходных параметров</param>
        private void ControlES(Output output)
        {
            MessageBox.Show(output.muni.ToString());
        }
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
