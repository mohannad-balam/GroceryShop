using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GroceryShop
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
        }

        int startPos = 0;
        private void progressTimer_Tick(object sender, EventArgs e)
        {
            startPos += 2;
            myProgress.Value = startPos;
            percentage.Text = startPos + " %";
            if(myProgress.Value == 100)
            {
                myProgress.Value = 0;
                progressTimer.Stop();
                Login login = new Login();
                login.Show();
                this.Hide();
            }
        }

        private void Splash_Load(object sender, EventArgs e)
        {
            progressTimer.Start();
        }
    }
}
