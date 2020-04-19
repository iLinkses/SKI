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

namespace Authorization
{
    public partial class AuthorizationForm : Form
    {
        public bool PasswordVerification = false;
        public bool IsAdmUser;

        SQLiteDB db = new SQLiteDB();
        public AuthorizationForm()
        {
            InitializeComponent();
            this.DialogResult = DialogResult.None;
            this.loginBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.loginBox.DrawMode = DrawMode.OwnerDrawFixed;
            this.loginBox.DrawItem += new DrawItemEventHandler(loginBox_DrawItem);
            this.loginBox.DropDownClosed += new EventHandler(loginBox_DropDownClosed);
            this.loginBox.MouseLeave += new EventHandler(loginBox_Leave);

            //Заполнение loginBox
            var Login = db.GetAuthorization();
            foreach (var login in Login)
            {
                loginBox.Items.Add(login.Login.ToString());
            }
        }

        private void loginBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) { return; }
            string text = loginBox.GetItemText(loginBox.Items[e.Index]);
            e.DrawBackground();
            using (SolidBrush br = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(text, e.Font, br, e.Bounds);
            }

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected && loginBox.DroppedDown)
            {
                if (TextRenderer.MeasureText(text, loginBox.Font).Width > loginBox.Width)
                {
                    toolTip1.Show(text, loginBox, e.Bounds.Right, e.Bounds.Bottom);
                }
                else
                {
                    toolTip1.Hide(loginBox);
                }
            }
            e.DrawFocusRectangle();
        }

        private void loginBox_MouseHover(object sender, EventArgs e)
        {
            if (loginBox.SelectedItem != null)
            {
                if (!loginBox.DroppedDown && TextRenderer.MeasureText(loginBox.SelectedItem.ToString(), loginBox.Font).Width > loginBox.Width)
                {
                    toolTip1.Show(loginBox.SelectedItem.ToString(), loginBox, loginBox.Location.X, loginBox.Location.Y);
                }
            }
        }

        private void loginBox_DropDownClosed(object sender, EventArgs e)
        {
            toolTip1.Hide(loginBox);
        }

        private void loginBox_Leave(object sender, EventArgs e)
        {
            toolTip1.Hide(loginBox);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (loginBox.SelectedIndex == 0)
            {
                IsAdmUser = false;
                this.DialogResult = DialogResult.OK;
                //Main main = new Main();
                //main.Show();
                ////this.Hide();
            }            

            //Проверка пароля
            if (loginBox.SelectedIndex == 1)
            {
                IsAdmUser = true;
                if (passBox.Text == "")
                {
                    MessageBox.Show("Введите пароль...", "Ошибка");
                    this.DialogResult = DialogResult.None;
                    return;
                }

                else
                {
                    string login = loginBox.SelectedItem.ToString(); ;
                    string password = passBox.Text.ToString();
                    var Pass = db.GetAuthorization().SingleOrDefault(p => p.Login == login);
                    if (Pass == null) 
                    {
                        this.DialogResult = DialogResult.None;
                        return;
                    }
                    if (password != Pass.Password)
                    {
                        MessageBox.Show("Вы ввели не правильный пароль...", "Ошибка");
                        this.DialogResult = DialogResult.None;
                    }
                    if (password == Pass.Password)
                    {
                        PasswordVerification = true;
                        this.DialogResult = DialogResult.OK;
                        //AdmnForm AdministratorForm = new AdmnForm();
                        //AdministratorForm.Show();
                        ////this.Hide();
                    }
                }
            }
        }

        private void loginBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            passBox.Clear();
            if (loginBox.SelectedIndex == 0) passBox.Enabled = false;
            else if (loginBox.SelectedIndex == 1) passBox.Enabled = true;
        }
    }
}
