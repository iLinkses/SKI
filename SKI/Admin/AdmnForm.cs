using System;
using System.Data;
using System.Windows.Forms;
using SQLiteDBConnection;
using System.Data.SQLite;
using System.Diagnostics;

namespace SKI
{
    public partial class AdmnForm : Form
    {
        private String dbFileName;
        private SQLiteConnection m_dbConn;
        private SQLiteCommand m_sqlCmd;
        private SQLiteCommandBuilder sqlCommandBuilder;
        private DataTable dTable;
        private BindingSource bindingSource = null;
        private SQLiteDataAdapter sqlAdapter = null;

        SQLiteDB db = new SQLiteDB();
        public AdmnForm()
        {
            InitializeComponent();
        }
        #region Обработка элементов формы
        private void AdmnForm_Load(object sender, EventArgs e)
        {
            //Заполнение CB
            var tabels = db.GetTablesNames();
            foreach (var tableName in tabels)
            {
                comboBox1.Items.Add(tableName.ToString());
            }
            dbFileName = "SKI.db";
            m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
            m_dbConn.Open();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDB(comboBox1.SelectedItem.ToString(), m_dbConn, m_sqlCmd);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                sqlAdapter.Update(dTable);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                sqlAdapter.Update(dTable);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }
        private void сменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (toolStripTextBox1.Text != string.Empty)
            {
                string login = "Admn";
                string ReSavePass = "UPDATE Authorization SET Password='" + toolStripTextBox1.Text.ToString() + "' WHERE Login ='" + login + "'";
                try
                {
                    SQLiteCommand cmd = m_dbConn.CreateCommand();
                    cmd.CommandText = ReSavePass;
                    int res = cmd.ExecuteNonQuery();
                    if (res > 0)
                        MessageBox.Show("Успешно.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Вы не ввели пароль.", "Ошибка");
                return;
            }
        }
        #endregion
        /// <summary>
        /// Загрузка таблиц в DGV
        /// </summary>
        /// <param name="nameOfTable">Имя таблицы</param>
        /// <param name="m_dbConn">Подключение</param>
        /// <param name="m_sqlCmd">Команда</param>
        void LoadDB(string nameOfTable, SQLiteConnection m_dbConn, SQLiteCommand m_sqlCmd)
        {
            try
            {
                if (m_dbConn.State != ConnectionState.Open)
                {
                    MessageBox.Show("Open connection with database");
                    return;
                }
                string sqlQuery = "SELECT * FROM " + nameOfTable;

                sqlAdapter = new SQLiteDataAdapter(sqlQuery, m_dbConn);
                sqlCommandBuilder = new SQLiteCommandBuilder(sqlAdapter);

                dTable = new DataTable(nameOfTable);
                sqlAdapter.Fill(dTable);

                bindingSource = new BindingSource();
                bindingSource.DataSource = dTable;
                dataGridView1.DataSource = bindingSource;
                dataGridView1.Columns[0].Visible = false;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void выйтиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void сменитьПользователяToolStripMenuItem_Click(object sender, EventArgs e)
        {
           var CurrProc = Process.GetCurrentProcess().ProcessName;
            Application.Exit();
            Process.Start(CurrProc+".exe");
        }
    }
}
