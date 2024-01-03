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
    public partial class Billing : Form
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader rdr;
        DbConnect dbConnect = new DbConnect();
        public Billing()
        {
            InitializeComponent();
            con = new SqlConnection(dbConnect.myConnection());
            loadItems();
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
            }
        }

        int stock = 0, key = 0;
        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvItems.Columns[e.ColumnIndex].Name;
            if (colName == "Select")
            {
                nameTb.Text = dgvItems.Rows[e.RowIndex].Cells[2].Value.ToString();
                priceTb.Text = dgvItems.Rows[e.RowIndex].Cells[4].Value.ToString();
                stock = int.Parse(dgvItems.Rows[e.RowIndex].Cells[3].Value.ToString());
                key = int.Parse(dgvItems.Rows[e.RowIndex].Cells[1].Value.ToString());
            }
        }
        int n = 0;
        double dgvTotal = 0;
        double amount;
        private void addToBillBtn_Click(object sender, EventArgs e)
        {
            if (qtyTb.Text == "" || int.Parse(qtyTb.Text) > stock || nameTb.Text == "")
            {
                MessageBox.Show("the quantity may be empty or greater than the stock");
            }
            else
            {
                double total = int.Parse(qtyTb.Text) * double.Parse(priceTb.Text);
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.CreateCells(dgvBill);
                newRow.Cells[0].Value = ++n;
                newRow.Cells[1].Value = key;
                newRow.Cells[2].Value = nameTb.Text;
                newRow.Cells[3].Value = priceTb.Text;
                newRow.Cells[4].Value = qtyTb.Text;
                newRow.Cells[5].Value = total;
                dgvBill.Rows.Add(newRow);
                dgvTotal += total;
                amount = dgvTotal;
                totalLbl.Text = "$" + dgvTotal;
                updateQty();
                reset();
            }
        }

        public void updateQty()
        {
            try
            {
                int newQty = stock - int.Parse(qtyTb.Text);
                string query = "UPDATE tbItem SET qty=@qty WHERE id = @id";
                con.Open();
                cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@qty", newQty);
                cmd.Parameters.AddWithValue("@id", key);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                MessageBox.Show("qty Updated Successfully!");
                con.Close();
                loadItems();
                //Clear();
            }
        }

        public void updateQtyPlus(int qty)
        {
            try
            {
                //int newQty = stock + qty;
                string query = "UPDATE tbItem SET qty= qty + @qty WHERE id = @id";
                con.Open();
                cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@qty", qty);
                cmd.Parameters.AddWithValue("@id", key);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                MessageBox.Show("qty Updated Successfully!");
                con.Close();
                loadItems();
                //Clear();
            }
        }

        private void reset()
        {
            nameTb.Clear();
            priceTb.Clear();
            qtyTb.Clear();
            clientNameTb.Clear();
        }

        private void printBtn_Click(object sender, EventArgs e)
        {
            if (clientNameTb.Text == "")
            {
                MessageBox.Show("Missing Information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                try
                {
                    string query = "INSERT INTO tbBill(empname,clientname,amount) VALUES(@empname,@clientname,@amount)";
                    if (MessageBox.Show("Are You Sure You Want To Save This Bill?", "Save Bill", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        con.Open();
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@empname", usernameLbl.Text);
                        cmd.Parameters.AddWithValue("@clientname", clientNameTb.Text);
                        cmd.Parameters.AddWithValue("@amount", amount);
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
                    MessageBox.Show("Bill Saved Successfully!");
                    con.Close();
                    if(printPreviewDialogBill.ShowDialog() == DialogResult.OK)
                    {
                        printDocumentBill.Print();
                    }
                    totalLbl.Text = "TOTAL";
                }
            }
        }

        private void Billing_Load(object sender, EventArgs e)
        {
            usernameLbl.Text = Login.empName;
        }
        private void printDocumentBill_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString("BALAM SUPERMARKET", new Font("Century Gothic", 25, FontStyle.Bold), Brushes.Red, new Point(230));
            e.Graphics.DrawString("Seller Name : " + usernameLbl.Text, new Font("Century Gothic", 20, FontStyle.Bold), Brushes.Blue, new Point(100, 100));
            e.Graphics.DrawString("Date : " + DateTime.Now.ToShortDateString(), new Font("Century Gothic", 20, FontStyle.Bold), Brushes.Blue, new Point(100, 130));
            e.Graphics.DrawString("Total Amount : " + amount, new Font("Century Gothic", 20, FontStyle.Bold), Brushes.Blue, new Point(100, 160));
            e.Graphics.DrawString("GaiaTech", new Font("Century Gothic", 20, FontStyle.Italic), Brushes.Red, new Point(270, 230));
            dgvBill.Rows.Clear();
        }

        private void logoutBtn_Click(object sender, EventArgs e)
        {
            Constants.Logout();
            this.Hide();
        }
        int quantityAdded = 0;
        private void dgvBill_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvBill.Columns[e.ColumnIndex].Name;
            quantityAdded = int.Parse(dgvBill.Rows[e.RowIndex].Cells[4].Value.ToString());
            key = int.Parse(dgvBill.Rows[e.RowIndex].Cells[1].Value.ToString());
            if (colName == "Delete")
            {
                if(MessageBox.Show("Remove From Cart?","",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dgvBill.Rows.Remove(dgvBill.Rows[e.RowIndex]);
                    updateQtyPlus(quantityAdded);
                }
            }
        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            reset();
        }
    }
}

