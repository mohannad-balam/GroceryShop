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
    public partial class Category : Form
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader rdr;
        DbConnect dbConnect = new DbConnect();
        public Category()
        {
            InitializeComponent();
            con = new SqlConnection(dbConnect.myConnection());
            loadCategories();
        }

        public void loadCategories()
        {
            try
            {
                dgvCategories.Rows.Clear();
                con.Open();
                string query = "SELECT * FROM tbCategory";
                cmd = new SqlCommand(query, con);
                rdr = cmd.ExecuteReader();
                int i = 0;
                while (rdr.Read())
                {
                    i++;
                    dgvCategories.Rows.Add(i, rdr["id"].ToString(), rdr["catname"].ToString());
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
        }
        private void savBtn_Click(object sender, EventArgs e)
        {
            if (nameTb.Text == "")
            {
                MessageBox.Show("Missing Information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                try
                {
                    string query = "INSERT INTO tbCategory(catname) VALUES(@catname)";
                    if (MessageBox.Show("Are You Sure You Want To Save This Category?", "Save Category", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        con.Open();
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@catname", nameTb.Text);
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
                    MessageBox.Show("Category Added Successfully!");
                    con.Close();
                    loadCategories();
                    Clear();
                }
            }
        }
        int key = 0;
        private void dgvCategories_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvCategories.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                try
                {
                    nameTb.Text = dgvCategories.Rows[e.RowIndex].Cells[2].Value.ToString();
                    updateBtn.Enabled = true;
                    savBtn.Enabled = false;
                    key = int.Parse(dgvCategories.Rows[e.RowIndex].Cells[1].Value.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (colName == "Delete")
            {
                try
                {
                    if (MessageBox.Show("Are You Sure You Want To Delete This Record?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        con.Open();
                        string query = "DELETE FROM tbCategory WHERE id = @id";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", dgvCategories.Rows[e.RowIndex].Cells[1].Value.ToString());
                        cmd.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show("Employee Has Been Deleted Successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        loadCategories();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void updateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "UPDATE tbCategory SET catname=@catname WHERE id = @id";
                if (MessageBox.Show("Are You Sure You Want To Update This Category?", "Update Category", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    con.Open();
                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@catname", nameTb.Text);
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
                loadCategories();
                Clear();
            }
        }

        private void itemsLbl_Click(object sender, EventArgs e)
        {
            Items items = new Items();
            items.Show();
            this.Hide();
        }

        private void employeeLbl_Click(object sender, EventArgs e)
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
