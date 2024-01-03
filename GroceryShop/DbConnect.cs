using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GroceryShop
{
    internal class DbConnect
    {
        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader rdr;
        DataTable dt;
        SqlDataAdapter sda;
        private string conString;
        public string myConnection()
        {
            conString = @"Data Source=APTX4869;Initial Catalog=GroceryShop;Integrated Security=True";
            return conString;
        }

        public DataTable getData(string query)
        {
            dt = new DataTable();
            sda = new SqlDataAdapter(query, conString);
            sda.Fill(dt);
            return dt;
        }
    }
}
