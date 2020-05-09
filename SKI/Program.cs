using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Authorization;

namespace SKI
{
    static class Program
    {
        public static bool _IsAdmUser;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AuthorizationForm AuthForm = new AuthorizationForm();
            AuthForm.ShowDialog();
            if (AuthForm.DialogResult == DialogResult.OK)
            {
                _IsAdmUser = AuthForm.IsAdmUser;
                Application.Run(new MainForm());
            }
            else return;
        }
    }
}
