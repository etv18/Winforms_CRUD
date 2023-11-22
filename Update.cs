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

namespace WinFormsApp1
{
    public partial class Update : Form
    {
        private bool f;
        string conStr;
        public Update(string conStr)
        {
            InitializeComponent();
            this.conStr = conStr;
        }

        private void Update_Load(object sender, EventArgs e)
        {
            
        }

        private void GetProductData(int productID)
        {
            string query = "SELECT item_name, design, color FROM products WHERE product_id = @productID";
            string name, design, color = string.Empty;
            
            using(SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(query, con);
                command.Parameters.AddWithValue("@productID", productID);
                try
                {
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if(reader.Read())
                    {
                        name = reader["item_name"].ToString();
                        design = reader["design"].ToString();
                        color = reader["color"].ToString();
                        txtName.Text = name;
                        txtDesign.Text = design;
                        txtColor.Text = color;
                    }

                    reader.Close();
                    
                }
                catch(Exception ex) 
                {
                    MessageBox.Show($"Error trying gettting the items data which corresponds [{productID}] as an ID: {ex.Message}");
                }
                
            }
        }

        private void UpdateData()
        {
            int id = int.Parse(txtProductid.Text);
            string newName = txtName.Text;
            string newDesign = txtDesign.Text;
            string newColor = txtColor.Text;
            DateTime updatedDate = DateTime.Now;

            string query = "UPDATE products SET item_name = @newName, design = @newDesign, color = @newColor, updated_date = @updatedDate WHERE product_id = @id";

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(query, con);

                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@newName", newName);
                command.Parameters.AddWithValue("@newDesign", newDesign);
                command.Parameters.AddWithValue("@newColor", newColor);
                command.Parameters.AddWithValue("@updatedDate", updatedDate);

                try
                {
                    con.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Data updated successfully !");
                    }
                    else
                    {
                        MessageBox.Show("Data was not updated.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating data: " + ex.Message);
                    f = true;
                }

                con.Close();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            int id = 0;
            if (txtProductid.Text != null)
            {
                try
                {
                    id = int.Parse(txtProductid.Text);
                    GetProductData(id);
                }
                catch 
                {
                    MessageBox.Show("Introduce a right format number.");
                    txtProductid.Text = "";
                }
            }
            else
            {
                MessageBox.Show("Introduce a right format number");
                txtProductid.Text = "";
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateData();
        }
    }
}
