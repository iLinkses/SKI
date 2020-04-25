using System;
using System.Reflection;
using System.Windows.Forms;
using SQLiteDBConnection;

namespace SKI
{
    public partial class AdmnForm : Form
    {
        SQLiteDB db = new SQLiteDB();
        public AdmnForm()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //Рефлексия. Использовение позднего связывания
                Assembly asm = Assembly.LoadFrom("SQLiteDBConnection.dll");
                Type t = asm.GetType("SQLiteDBConnection.SQLiteDB", true, true);
                // создаем экземпляр класса Program
                object obj = Activator.CreateInstance(t);
                // получаем метод GetResult
                MethodInfo method = t.GetMethod("Get" + comboBox1.SelectedItem.ToString());
                // вызываем метод, передаем ему значения для параметров и получаем результат
                object result = method.Invoke(obj, new object[] {});

                dataGridView1.DataSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AdmnForm_Load(object sender, EventArgs e)
        {
            var tabels = db.GetTablesNames();
            foreach (var tableName in tabels)
            {
                comboBox1.Items.Add(tableName.ToString());
            }
        }
    }
}
