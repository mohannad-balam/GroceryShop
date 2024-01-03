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
    public partial class Items : Form
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader rdr;
        DbConnect dbConnect = new DbConnect();
        public Items()
        {
            InitializeComponent();
            con = new SqlConnection(dbConnect.myConnection());
            getCategory();
            loadItems();
        }

        private void employeesLbl_Click(object sender, EventArgs e)
        {
            Employees employee = new Employees();
            employee.Show();
            this.Hide();
        }

        public void loadItems()
        {
            try
            {
                dgvItems.Rows.Clear();
                con.Open();
                string query = "SELECT * FROM vwItems";
                cmd = new SqlCommand(query, con);
                rdr = cmd.ExecuteReader();
                int i = 0;
                while (rdr.Read())
                {
                    i++;
                    dgvItems.Rows.Add(i, rdr["id"].ToString(), rdr["name"].ToString(), rdr["qty"].ToString(), rdr["price"].ToString(), rdr["catname"].ToString());
                }
                rdr.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
                savBtn.Enabled = true;
                updateBtn.Enabled = false;
            }
        }

        public void getCategory()
        {
            string query = "SELECT * FROM tbCategory";
            con.Open();
            cmd = new SqlCommand(query, con);
            categoryCb.DisplayMember = dbConnect.getData(query).Columns["catname"].ToString();
            categoryCb.ValueMember = dbConnect.getData(query).Columns[0].ToString();
            categoryCb.DataSource = dbConnect.getData(query);
            con.Close();
        }

        public void Clear()
        {
            nameTb.Clear();
            priceTb.Clear();
            quantityTb.Clear();
        }
        private void savBtn_Click(object sender, EventArgs e)
        {
            if (nameTb.Text == "" || quantityTb.Text == "" || priceTb.Text == "")
            {
                MessageBox.Show("Missing Information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                try
                {
                    string query = "INSERT INTO tbItem(name,qty,price,catid) VALUES (@name,@qty,@price,@catid)";
                    if (MessageBox.Show("Are You Sure You Want To Save This Employee?", "Save Employee", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        con.Open();
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@name", nameTb.Text);
                        cmd.Parameters.AddWithValue("@qty", quantityTb.Text);
                        cmd.Parameters.AddWithValue("@price", double.Parse(priceTb.Text));
                        cmd.Parameters.AddWithValue("@catid", int.Parse(categoryCb.SelectedValue.ToString()));
                        cmd.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show("Item Added Successfully!");
                        loadItems();
                        Clear();
                    }
                }
                catch (Exception ex)
                {
                    con.Close();
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    con.Close();
                }
            }
        }

        int key = 0;
        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvItems.Columns[e.ColumnIndex].Name;
            if(colName == "Edit")
            {
                nameTb.Text = dgvItems.Rows[e.RowIndex].Cells[2].Value.ToString();
                quantityTb.Text = dgvItems.Rows[e.RowIndex].Cells[3].Value.ToString();
                priceTb.Text = dgvItems.Rows[e.RowIndex].Cells[4].Value.ToString();
                categoryCb.Text = dgvItems.Rows[e.RowIndex].Cells[5].Value.ToString();
                key = int.Parse(dgvItems.Rows[e.RowIndex].Cells[1].Value.ToString());
                updateBtn.Enabled = true;
                savBtn.Enabled = false;
            }
            else if(colName == "Delete")
            {
                try
                {
                    if (MessageBox.Show("Are You Sure You Want To Delete This Record?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        con.Open();
                        string query = "DELETE FROM tbItem WHERE id = @id";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", dgvItems.Rows[e.RowIndex].Cells[1].Value.ToString());
                        cmd.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show("Item Has Been Deleted Successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        loadItems();
                    }
                }
                catch (Exception ex)
                {
                    con.Close();
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void updateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "UPDATE tbItem SET name=@name, qty=@qty, price=@price, catid=@catid WHERE id = @id";
                if (MessageBox.Show("Are You Sure You Want To Update This Item?", "Update Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    con.Open();
                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@name", nameTb.Text);
                    cmd.Parameters.AddWithValue("@qty", quantityTb.Text);
                    cmd.Parameters.AddWithValue("@price", priceTb.Text);
                    cmd.Parameters.AddWithValue("@catid", int.Parse(categoryCb.SelectedValue.ToString()));
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
                loadItems();
                Clear();
            }
        }

        private void categoryBtn_Click(object sender, EventArgs e)
        {
            Category category = new Category();
            category.Show();
            this.Hide();
        }

        private void logoutBtn_Click(object sender, EventArgs e)
        {
            Constants.Logout();
            this.Hide();
        }
    }
}
