using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GroceryShop
{
    public partial class Login : Form
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader rdr;
        DbConnect dbConnect = new DbConnect();

        public static string empName = "";
        public Login()
        {
            InitializeComponent();
            con = new SqlConnection(dbConnect.myConnection());
        }
        private void closePic_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Close Applicarion ?", "Close App", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void loginAsAdminBtn_Click(object sender, EventArgs e)
        {
            if (usernameTb.Text == "admin" && passwordTb.Text == "admin")
            {
                MessageBox.Show("Welcome Admin");
                Employees employees = new Employees();
                employees.Show();
                this.Hide();
            }else
            {
                MessageBox.Show("invalid admin credentials");
                return;
            }
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                cmd = new SqlCommand("SELECT COUNT(*) FROM tbEmployee WHERE name=@name AND password=@password",con);
                cmd.Parameters.AddWithValue("@name", usernameTb.Text);
                cmd.Parameters.AddWithValue("@password", passwordTb.Text);
                int c = int.Parse(cmd.ExecuteScalar().ToString());
                if (c == 1)
                {
                    empName = usernameTb.Text;
                    Billing billing = new Billing();
                    billing.Show();
                    this.Hide();
                    con.Close();
                }
                else
                {
                    MessageBox.Show("invalid credential");
                    con.Close();
                }
                con.Close();
            }
            catch(Exception ex)
            {
                con.Close();
                MessageBox.Show(ex.Message);
            }
        }
    }
}
