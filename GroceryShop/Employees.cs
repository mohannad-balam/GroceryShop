using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace GroceryShop
{
    public partial class Employees : Form
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader rdr;
        DbConnect dbConnect = new DbConnect();
        public Employees()
        {
            InitializeComponent();
            con = new SqlConnection(dbConnect.myConnection());
        }

        public void loadEmployees()
        {
            try
            {
                dgvEmployees.Rows.Clear();
                con.Open();
                string query = "SELECT * FROM tbEmployee WHERE name LIKE '%" +searchTb.Text+ "%'";
                cmd = new SqlCommand(query, con);
                rdr = cmd.ExecuteReader();
                int i = 0;
                while (rdr.Read())
                {
                    i++;
                    dgvEmployees.Rows.Add(i, rdr["id"].ToString(), rdr["name"].ToString(), rdr["phone"].ToString(), rdr["address"].ToString(), rdr["password"].ToString());
                }
                rdr.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Clear()
        {
            nameTb.Clear();
            phoneTb.Clear();
            addressTb.Clear();
            passwordTb.Clear();
        }

        private void itemsLbl_Click(object sender, EventArgs e)
        {
            Items items = new Items();
            items.Show();
            this.Hide();
        }

        private void savBtn_Click(object sender, EventArgs e)
        {
            if(nameTb.Text == "" || phoneTb.Text == "" || addressTb.Text == "" || passwordTb.Text == "")
            {
                MessageBox.Show("Missing Information!","Error",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                try
                {
                    string query = "INSERT INTO tbEmployee(name,phone,address,password) VALUES(@name,@phone,@address,@password)";
                    if (MessageBox.Show("Are You Sure You Want To Save This Employee?","Save Employee",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        con.Open();
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@name", nameTb.Text);
                        cmd.Parameters.AddWithValue("@phone", phoneTb.Text);
                        cmd.Parameters.AddWithValue("@address", addressTb.Text);
                        cmd.Parameters.AddWithValue("@password", passwordTb.Text);
                        cmd.ExecuteNonQuery();
                    }
                }catch (Exception ex)
                {
                    con.Close();
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    MessageBox.Show("Employee Added Successfully!");
                    con.Close();
                    loadEmployees();
                    Clear();
                }
            }
        }

        private void Employees_Load(object sender, EventArgs e)
        {
            loadEmployees();
        }

        int key = 0;
        private void dgvEmployees_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvEmployees.Columns[e.ColumnIndex].Name;
            if(colName == "Edit")
            {
                try
                {
                    nameTb.Text = dgvEmployees.Rows[e.RowIndex].Cells[2].Value.ToString();
                    phoneTb.Text = dgvEmployees.Rows[e.RowIndex].Cells[3].Value.ToString();
                    addressTb.Text = dgvEmployees.Rows[e.RowIndex].Cells[4].Value.ToString();
                    passwordTb.Text = dgvEmployees.Rows[e.RowIndex].Cells[5].Value.ToString();
                    updateBtn.Enabled = true;
                    savBtn.Enabled = false;
                    key = int.Parse(dgvEmployees.Rows[e.RowIndex].Cells[1].Value.ToString());
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if(colName == "Delete")
            {
                try
                {
                    if (MessageBox.Show("Are You Sure You Want To Delete This Record?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        con.Open();
                        string query = "DELETE FROM tbEmployee WHERE id = @id";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", dgvEmployees.Rows[e.RowIndex].Cells[1].Value.ToString());
                        cmd.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show("Employee Has Been Deleted Successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        loadEmployees();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void searchTb_TextChanged(object sender, EventArgs e)
        {
            loadEmployees();
        }

        private void updateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "UPDATE tbEmployee SET name=@name, phone=@phone, address=@address, password=@password WHERE id = @id";
                if (MessageBox.Show("Are You Sure You Want To Update This Employee?", "Update Employee", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    con.Open();
                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@name", nameTb.Text);
                    cmd.Parameters.AddWithValue("@phone", phoneTb.Text);
                    cmd.Parameters.AddWithValue("@address", addressTb.Text);
                    cmd.Parameters.AddWithValue("@password", passwordTb.Text);
                    cmd.Parameters.AddWithValue("@id", key);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                MessageBox.Show("Employee Updated Successfully!");
                con.Close();
                loadEmployees();
                Clear();
            }
        }

        private void lblCats_Click(object sender, EventArgs e)
        {
            Category category = new Category();
            category.Show();
            this.Hide();
        }

        private void searchTb_TextChanged_1(object sender, EventArgs e)
        {
            loadEmployees();
        }

        private void employeeBtn_Click(object sender, EventArgs e)
        {
            Employees employees = new Employees();
            employees.Show();
            this.Hide();
        }

        private void logoutBtn_Click(object sender, EventArgs e)
        {
            Constants.Logout();
            this.Hide();
        }
    }
}
