using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using ZedGraph;

/*********************************************************************
**                                                                  **
**                   Расчет реактора полимеризации.                 **
**                                                                  **
**                           Версия 1.0                             **
**                            2020 год                              **
**                                                                  **
*********************************************************************/

namespace SKI
{
    class Сalculation
    {
        public MathModel Owner;

        static double h;//0.1;
        static double tau;//3;
        static int n;//Convert.ToInt32(tau / h);

        #region Расчет СКИ
        /// <summary>
        /// Расчет СКИ
        /// </summary>
        /// <param name="mvx">Концентрация изопрена в шихте</param>
        /// <param name="Tvx">Температура шихты</param>
        /// <param name="G">Расход шихты</param>
        /// <param name="Gk">Расход каталитического комплекса</param>
        /// <param name="Ghl">Расход хладагента</param>
        /// <param name="Thl">Температура ладагента</param>
        /// <param name="ch2vx">Концентрация водорода в шихте</param>
        public void Calculation(double mvx, double Tvx, double G, double Gk, double Ghl, double Thl, double ch2vx)
        {
            MathModel main = Owner as MathModel;
            h = Convert.ToDouble(main.numericUpDown15.Value);
            tau = Convert.ToDouble(main.numericUpDown16.Value);
            n = Convert.ToInt32(tau / h);

            //---Вектор входных переменных---
            double V = GetParameters("Входной", 14);//.AsEnumerable().Where(p => p.Field<long>("ID") == 14).Select(p => p.Field<double>("Value")).Single();//4.1; //Объем реактора
            double alfa = GetParameters("Входной", 15);//.AsEnumerable().Where(p => p.Field<long>("ID") == 15).Select(p => p.Field<double>("Value")).Single();//1.22; //Активность катализатора
            double beta = GetParameters("Входной", 16);//.AsEnumerable().Where(p => p.Field<long>("ID") == 16).Select(p => p.Field<double>("Value")).Single();//0.3;  //Коэффициент, учитывающий примеси в шихте
            double cp = GetParameters("Входной", 17);//.AsEnumerable().Where(p => p.Field<long>("ID") == 17).Select(p => p.Field<double>("Value")).Single();//0.55; //Теплоемкость реакционной смеси
            double A = GetParameters("Входной", 18);//.AsEnumerable().Where(p => p.Field<long>("ID") == 18).Select(p => p.Field<double>("Value")).Single();//0.5; //Коэффициент, учитывающий гидродинамические характеристики реактора
            double s = GetParameters("Входной", 19);//.AsEnumerable().Where(p => p.Field<long>("ID") == 19).Select(p => p.Field<double>("Value")).Single();//12; //Поверхность теплосъема
            double chl = GetParameters("Входной", 20);//.AsEnumerable().Where(p => p.Field<long>("ID") == 20).Select(p => p.Field<double>("Value")).Single();//0.6; //теплоемкость хладагента
            double ro = GetParameters("Входной", 21);//.AsEnumerable().Where(p => p.Field<long>("ID") == 21).Select(p => p.Field<double>("Value")).Single();//630; //Плотность реакционной смеси
            double q = GetParameters("Входной", 22);//.AsEnumerable().Where(p => p.Field<long>("ID") == 22).Select(p => p.Field<double>("Value")).Single();//263; //Тепловой эффект реакции

            //Console.WriteLine($"{V} {alfa} {beta} {cp} {A} {s} {chl} {ro} {q}") ;

            //---Вектор параметров модели---
            //double E = 10000;
            double R = GetParameters("Константа", 8);//8.314; //Универсальная газовая постоянная
            double k0 = GetParameters("Константа", 9);//500000000; //Константа для расчета скорости реакции полимеризаии
            double k1 = GetParameters("Константа", 10);//10.0; //Константа для расчета скорости реакции полимеризаии
            double k2 = GetParameters("Константа", 11);//1.3; //Константа для расчета скорости реакции полимеризаии
            double u = GetParameters("Константа", 12);//40; //Коэффициент теплопередачи
            double kh = GetParameters("Константа", 13);//1.56; //Коэффициент для расчета реакции переноса цепи по водороду

            double[] tp = new double[n + 1];
            double[] micp0 = new double[n + 1]; //Среднеинтегральные значения мономера
            double[] Tcp0 = new double[n + 1]; //Среднеинтегральные значения температуры
            double[] m0 = new double[n + 1]; //---!!!Нигде не используется!!!---
            double[] T0 = new double[n + 1]; //---!!!Нигде не используется!!!---
            double[] Ki = new double[n + 1]; //Для расчета скорости реакции полимеризации
            double[] r = new double[n + 1]; //Скорость реакции полимеризации
            double[] ch20 = new double[n + 1];
            double[] rh2 = new double[n + 1];
            double[] rd = new double[n + 1]; //Скорость реакции образования димеров

            //---Выходные параметры---
            double[] muni = new double[n + 1]; //Вязкость по Муни
            double[] pl = new double[n + 1]; //Пластичность по Карреру
            double[] pm = new double[n + 1]; //Потери массы
            double[] m = new double[n + 1]; //Концентрация полимера
            double[] T = new double[n + 1]; // Выходная температура смеси
            double[] ch2 = new double[n + 1]; //Выходная концентрация водорода
            double[] d = new double[n + 1]; //Концентрация димеров

            m[0] = 0;
            d[0] = 0;
            T[0] = 0 + 273;
            m0[0] = 0;
            micp0[0] = 0;
            Tcp0[0] = 0;
            T0[0] = Tvx;
            ch2[0] = 20;
            ch20[0] = 0;

            for (int j = 1; j <= n; j++)
            {
                double no;
                if (alfa * Gk / G > beta)
                {
                    no = alfa * Gk / G - beta;
                }
                else no = 0;
                tp[j] = h * j;
                micp0[j] = (micp0[j - 1] + A * mvx) / (1 + A);
                Tcp0[j] = (Tcp0[j - 1] + A * Tvx) / (1 + A);
                //m0[j] = m0[j - 1] + h * G * (mvx - m0[j - 1]);
                //T0[j] = T0[j - 1] + h * (G * cp * (Tvx - T0[j - 1]) - (2 * u * s * Ghl * chl * (Tcp0[j] - Thl)) / (2 * Ghl * chl + u * s));
                Ki[j] = k0 * Math.Exp(-Math.Pow((k1 * (mvx - micp0[j])), k2));
                r[j] = Ki[j] * no * Math.Sqrt(micp0[j]) * Math.Exp(-21.5 / R * Tcp0[j] + 273);
                m[j] = m[j - 1] + h * G * (0.9 * mvx - m[j - 1]) - h * V * r[j] * ro;
                T[j] = T[j - 1] + h * (G * cp * (Tvx - T[j - 1]) - (2 * u * s * Ghl * chl * Tcp0[j]) / (2 * Ghl * chl + u * s) + V * ro * q * r[j]);

                ch20[j] = ch20[j - 1] + h * G / 100 * (ch2vx - ch20[j - 1]);
                rh2[j] = kh * ch20[j];
                ch2[j] = ch2[j - 1] + h * G * (ch2vx - ch2[j - 1]) - h * V * ro * rh2[j];

                rd[j] = Gk * (4.2e13 * ch2[j] + 2.4e14) * micp0[j] * Math.Exp(-280 / R * Tcp0[j] + 273) / G;
                d[j] = d[j - 1] + h * G * (0.1 * Gk * mvx - d[j - 1]) - h * V * rd[j] * ro;

                muni[j] = 20 * Math.Pow(micp0[j], 0.28) * Math.Exp(25 / Tcp0[j]);
                pl[j] = 20 * Math.Exp(50 / Tcp0[j]) / muni[j];
                pm[j] = Gk * d[j - 1] / m[j];

                if (ch2[j] < 0) ch2[j] = 0;
                if (d[j] < 0) d[j] = 0;
                if (m[j] < 0) m[j] = 0;
                if (pm[j] < 0) pm[j] = 0;
            }

            //if (main.tabControl1.SelectedTab == main.tabPage10)
            //{
            //    main.dataGridView1.Rows[15].Cells["Value"].Value = main.textBox2.Text = Convert.ToString(string.Format("{0:N1}", T[n] - 200));
            //    main.dataGridView1.Rows[11].Cells["Value"].Value = main.textBox3.Text = Convert.ToString(string.Format("{0:N1}", muni[n]));
            //    main.dataGridView1.Rows[12].Cells["Value"].Value = main.textBox4.Text = Convert.ToString(string.Format("{0:N3}", pl[n]));
            //    main.dataGridView1.Rows[13].Cells["Value"].Value = main.textBox5.Text = Convert.ToString(string.Format("{0:N3}", pm[n]));
            //    main.dataGridView1.Rows[14].Cells["Value"].Value = Convert.ToString(string.Format("{0:N1}", m[n]));
            //    main.dataGridView1.Rows[16].Cells["Value"].Value = Convert.ToString(string.Format("{0:N3}", ch2[n]));
            //    main.dataGridView1.Rows[17].Cells["Value"].Value = Convert.ToString(string.Format("{0:N2}", d[n]));
            //}
            //if (main.tabControl1.SelectedTab == main.tabPage1)
            //{
            //    main.textBox1.Text = Convert.ToString(string.Format("{0:N1}", muni[n]));
            //    main.textBox9.Text = Convert.ToString(string.Format("{0:N3}", pl[n]));
            //    main.textBox10.Text = Convert.ToString(string.Format("{0:N3}", pm[n]));
            //    main.textBox11.Text = Convert.ToString(string.Format("{0:N1}", m[n]));
            //    main.textBox12.Text = Convert.ToString(string.Format("{0:N1}", T[n] - 200));
            //    main.textBox13.Text = Convert.ToString(string.Format("{0:N3}", ch2[n]));
            //    main.textBox14.Text = Convert.ToString(string.Format("{0:N2}", d[n]));
            //}

            main.dataGridView1.Rows[11].Cells["Value"].Value = main.textBox3.Text = main.textBox1.Text = Convert.ToString(string.Format("{0:N1}", muni[n]));
            main.dataGridView1.Rows[12].Cells["Value"].Value = main.textBox4.Text = main.textBox9.Text = Convert.ToString(string.Format("{0:N3}", pl[n]));
            main.dataGridView1.Rows[13].Cells["Value"].Value = main.textBox5.Text = main.textBox10.Text = Convert.ToString(string.Format("{0:N3}", pm[n]));
            main.dataGridView1.Rows[14].Cells["Value"].Value = main.textBox11.Text = Convert.ToString(string.Format("{0:N1}", m[n]));
            main.dataGridView1.Rows[15].Cells["Value"].Value = main.textBox2.Text = main.textBox12.Text = Convert.ToString(string.Format("{0:N1}", T[n] - 200));
            main.dataGridView1.Rows[16].Cells["Value"].Value = main.textBox13.Text = Convert.ToString(string.Format("{0:N3}", ch2[n]));
            main.dataGridView1.Rows[17].Cells["Value"].Value = main.textBox14.Text = Convert.ToString(string.Format("{0:N2}", d[n]));

            //Уменьшение выходной температура на 200 для графика
            double[] TClone = new double[n + 1];
            Array.Copy(T, TClone, n + 1);
            for (int j = 0; j <= n; j++)
            {
                TClone[j] = TClone[j] - 200;
            }

            DrawGraph(muni, pl, pm, m, TClone, ch2, d);
        }
        #endregion

        #region Получения значения параметра
        /// <summary>
        /// Получает значение параметра по ID
        /// </summary>
        /// <param name="type">Тип параметра (Управляющий/Входной/Константа)</param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private double GetParameters(string type, int ID)
        {
            double parameter=0;
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
                string Command = "select TP.ID, TP. Value " +
                                 "from Technological_Parameters TP " +
                                 "where TP.Type = '" + type + "' " +
                                 "and TP.ID = " + ID;
                SQLiteDataAdapter sqlAdapter = new SQLiteDataAdapter(Command, m_dbConn);
                SQLiteCommandBuilder sqlCommandBuilder = new SQLiteCommandBuilder(sqlAdapter);

                
                sqlAdapter.Fill(dTable);
                parameter = double.Parse(dTable.Rows[0]["Value"].ToString());
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            m_dbConn.Close();
            return parameter;
        }
        #endregion
        #region Добавление графиков
        /// <summary>
        /// Добавление графиков
        /// </summary>
        /// <param name="muni">Муни</param>
        /// <param name="pl">Пластичность по Карреру</param>
        /// <param name="pm">Потери массы</param>
        /// <param name="m">Концентрация полимера</param>
        /// <param name="T">Выходная температура смеси</param>
        /// <param name="ch2">Выходная концентрация водорода</param>
        /// <param name="d">Концентрация димеров</param>
        private void DrawGraph(double[] muni, double[] pl, double[] pm, double[] m, double[] T, double[] ch2, double[] d)
        {
            MathModel main = Owner as MathModel;
            // Получим панель для рисования
            GraphPane paneMuni = main.zedGraphControl1.GraphPane;
            // Изменим текст заголовка графика
            paneMuni.Title.Text = "";
            // Изменим тест надписи по оси X
            paneMuni.XAxis.Title.Text = "мин.";
            // Изменим текст по оси Y
            paneMuni.YAxis.Title.Text = "ед.";
            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            paneMuni.CurveList.Clear();
            MinMaxGraphs(paneMuni, 1);
            Graphs(paneMuni, muni);

            GraphPane panePl = main.zedGraphControl2.GraphPane;
            panePl.Title.Text = "";
            panePl.XAxis.Title.Text = "мин.";
            panePl.YAxis.Title.Text = "ед.";
            panePl.CurveList.Clear();
            MinMaxGraphs(panePl, 2);
            Graphs(panePl, pl);

            GraphPane panePm = main.zedGraphControl3.GraphPane;
            panePm.Title.Text = "";
            panePm.XAxis.Title.Text = "мин.";
            panePm.YAxis.Title.Text = "ед.";
            panePm.CurveList.Clear();
            MinMaxGraphs(panePm, 3);
            Graphs(panePm, pm);

            GraphPane paneM = main.zedGraphControl4.GraphPane;
            paneM.Title.Text = "";
            paneM.XAxis.Title.Text = "мин.";
            paneM.YAxis.Title.Text = "%";
            paneM.CurveList.Clear();
            MinMaxGraphs(paneM, 4);
            Graphs(paneM, m);

            GraphPane paneT = main.zedGraphControl5.GraphPane;
            paneT.Title.Text = "";
            paneT.XAxis.Title.Text = "мин.";
            paneT.YAxis.Title.Text = "°C";
            paneT.CurveList.Clear();
            MinMaxGraphs(paneT, 5);
            Graphs(paneT, T);

            GraphPane paneCh2 = main.zedGraphControl6.GraphPane;
            paneCh2.Title.Text = "";
            paneCh2.XAxis.Title.Text = "мин.";
            paneCh2.YAxis.Title.Text = "%";
            paneCh2.CurveList.Clear();
            MinMaxGraphs(paneCh2, 6);
            Graphs(paneCh2, ch2);

            GraphPane paneD = main.zedGraphControl7.GraphPane;
            paneD.Title.Text = "";
            paneD.XAxis.Title.Text = "мин.";
            paneD.YAxis.Title.Text = "%";
            paneD.CurveList.Clear();
            MinMaxGraphs(paneD, 7);
            Graphs(paneD, d);

            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            // В противном случае на рисунке будет показана только часть графика, 
            // которая умещается в интервалы по осям, установленные по умолчанию
            main.zedGraphControl1.AxisChange();
            main.zedGraphControl2.AxisChange();
            main.zedGraphControl3.AxisChange();
            main.zedGraphControl4.AxisChange();
            main.zedGraphControl5.AxisChange();
            main.zedGraphControl6.AxisChange();
            main.zedGraphControl7.AxisChange();
            // Обновляем график
            main.zedGraphControl1.Invalidate();
            main.zedGraphControl2.Invalidate();
            main.zedGraphControl3.Invalidate();
            main.zedGraphControl4.Invalidate();
            main.zedGraphControl5.Invalidate();
            main.zedGraphControl6.Invalidate();
            main.zedGraphControl7.Invalidate();
        }
        #endregion

        //Сделать отрисовку главного графика во весь экран (скрыть масштабирование пороговых диапазонов)
        //Пробовал отрисовывать главный график, не понравилось :)
        #region Отрисовка графика
        /// <summary>
        /// Отрисовка графика
        /// </summary>
        /// <param name="pane"> Панель для рисования</param>
        /// <param name="parameter">Массив точек</param>
        private void Graphs(GraphPane pane, double[] parameter)
        {
            PointPairList points = new PointPairList();
            // Заполняем список точек
            for (int j = 1; j <= n; j++)
            {
                // добавим в список точку
                points.Add(h * j * 60, parameter[j]);
            }
            // Создадим кривую,
            LineItem myCurve = pane.AddCurve("", points, Color.Red, SymbolType.Circle);
            // толщина линии 2 пикселя
            myCurve.Line.Width = 3;
            // точки в виде сплошных кругов
            myCurve.Symbol.Fill.Type = FillType.Solid;
            // определяем размер точек
            myCurve.Symbol.Size = 7;
            // Включим сглаживание
            myCurve.Line.IsSmooth = true;
            // Устанавливаем шаг оси Х
            pane.XAxis.Scale.MajorStep = 10;

            // Устанавливаем интересующий нас интервал по оси X
            pane.XAxis.Scale.Min = h * 60;
            pane.XAxis.Scale.Max = h * n * 60;

            ////Интервал по Y
            //var ArrWitout0 = parameter.Where(t => t != 0);
            //int znam;
            //if (ArrWitout0.Min() > 10)
            //{
            //    znam = 10;
            //}
            //else
            //{
            //    znam = 2;
            //}
            //if(ArrWitout0.First() < ArrWitout0.Last())
            //{
            //    pane.YAxis.Scale.Min = ArrWitout0.Min() - Math.Abs(ArrWitout0.Min() / znam);
            //    pane.YAxis.Scale.Max = ArrWitout0.Max() + Math.Abs(ArrWitout0.Min() / znam);
            //}
            //else
            //{
            //    pane.YAxis.Scale.Min = ArrWitout0.Min() - Math.Abs(ArrWitout0.Max() / znam);
            //    pane.YAxis.Scale.Max = ArrWitout0.Max() + Math.Abs(ArrWitout0.Max() / znam);
            //}

            // Включаем отображение сетки напротив крупных рисок по оси X
            pane.XAxis.MajorGrid.IsVisible = true;

            // Задаем вид пунктирной линии для крупных рисок по оси X:
            // Длина штрихов равна 10 пикселям, ... 
            pane.XAxis.MajorGrid.DashOn = 5;
            // затем 5 пикселей - пропуск
            pane.XAxis.MajorGrid.DashOff = 5;

            // Включаем отображение сетки напротив крупных рисок по оси Y
            pane.YAxis.MajorGrid.IsVisible = true;

            // Аналогично задаем вид пунктирной линии для крупных рисок по оси Y
            pane.YAxis.MajorGrid.DashOn = 5;
            pane.YAxis.MajorGrid.DashOff = 5;

            // Включаем отображение сетки напротив мелких рисок по оси X
            pane.YAxis.MinorGrid.IsVisible = true;

            // Задаем вид пунктирной линии для крупных рисок по оси Y: 
            // Длина штрихов равна одному пикселю, ... 
            pane.YAxis.MinorGrid.DashOn = 1;

            // затем 2 пикселя - пропуск
            pane.YAxis.MinorGrid.DashOff = 2;

            // Включаем отображение сетки напротив мелких рисок по оси Y
            pane.XAxis.MinorGrid.IsVisible = true;

            // Аналогично задаем вид пунктирной линии для крупных рисок по оси Y
            pane.XAxis.MinorGrid.DashOn = 1;
            pane.XAxis.MinorGrid.DashOff = 2;
        }
        #endregion
        #region Отрисовка пороговых диапазонов
        private void MinMaxGraphs(GraphPane pane, int ID)
        {
            MathModel main = Owner as MathModel;
            List<MathModel.MinMax> minmax = new List<MathModel.MinMax>();
            minmax = main.GetMin_Max(ID);

            PointPairList MinPoints = new PointPairList();
            for (int j = 1; j <= n; j++)
            {
                MinPoints.Add(h * j * 60, minmax[0].min);
            }

            PointPairList MaxPoints = new PointPairList();
            for (int j = 1; j <= n; j++)
            {
                MaxPoints.Add(h * j * 60, minmax[0].max);
            }
            LineItem MinCurve = pane.AddCurve("", MinPoints, Color.Blue, SymbolType.None);
            LineItem MaxCurve = pane.AddCurve("", MaxPoints, Color.Blue, SymbolType.None);


            MinCurve.Line.Style = DashStyle.Dash;
            MinCurve.Line.Width = 3;
            MaxCurve.Line.Style = DashStyle.Dash;
            MaxCurve.Line.Width = 3;
        }
        #endregion
    }
}

