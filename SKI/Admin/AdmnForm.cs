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
    public partial class AdmnForm : Form
    {
        SQLiteDB db = new SQLiteDB();
        public AdmnForm()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

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
