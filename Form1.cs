

using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private string dbUser, dbPassword, conStr; 
        public Form1()
        {
            InitializeComponent();
            StringConnector(); //Method to get de credentials and put it into the string connector.
        }

        private void GetCredentialsDB()
        {
            string env_v = "C_DTLS"; //Name of the enviroment variable.
            try
            {
                string configFilePath = Environment.GetEnvironmentVariable(env_v);

                if (File.Exists(configFilePath))
                {
                    string[] lines = File.ReadAllLines(configFilePath);

                    if (lines.Length > 0)
                    {
                        string[] cdtls = lines[0].Split(',');

                        if (cdtls.Length == 2)
                        {
                            dbUser = cdtls[0].Trim();
                            dbPassword = cdtls[1].Trim();

                        }
                        else MessageBox.Show("The file it doesn't have the expected format.");
                    }
                    else MessageBox.Show("The file is empty");

                }
                else MessageBox.Show($"The enviroment variable {env_v} is not set or the file doesn't exists in the especified path.");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to read the file: " + ex.Message);
            }
        }

        private void StringConnector()
        {
            GetCredentialsDB();
            string connectionString = $"Data Source=LAPTOP-68IMB34E;Initial Catalog=storageDBO;User ID={dbUser};Password={dbPassword};Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            conStr = connectionString;
        }

        public void CleanBoxes()
        {
            txtColor.Text = txtDesign.Text = txtProductid.Text = txtColor.Text = txtName.Text = string.Empty;
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            InsertData();
            CleanBoxes();
            BindData();
        }

        private void InsertData()
        {
            //Create a connection with str conector which is in database properties.
            //This is a good practice of how to connect to db avoiding sql injection.
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();

                int id = int.Parse(txtProductid.Text);
                string name = txtName.Text;
                string color = txtColor.Text;
                string design = txtDesign.Text;

                string query = "INSERT INTO products (product_id, item_name, color, design, date) VALUES (@id,@name,@design,@color, GETDATE())";
                SqlCommand command = new SqlCommand(query, con);

                // Add parameters to the query
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@design", design);
                command.Parameters.AddWithValue("@color", color);

                command.ExecuteNonQuery();
                MessageBox.Show("Succesfully Inserted");

                con.Close();
            }
        }

        public void BindData()
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = "SELECT * FROM products";
                SqlCommand command = new SqlCommand(query, con);

                try
                {
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvData.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BindData();
        }
        private bool f;
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Update formularioUpdate = new Update(conStr);
            formularioUpdate.Show();
        }
        private void DeleteData()
        {
            int id = int.Parse(txtProductid.Text);
            string query = "DELETE products WHERE product_id = @id";

            using(SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(query, con);
                command.Parameters.AddWithValue("@id", id);

                try
                {
                    con.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Data deleted successfully !");
                    }
                    else
                    {
                        MessageBox.Show("No data was deleted. Check if the product ID exists.");
                    }
                }
                catch (Exception ex) 
                {
                    MessageBox.Show("Error deleting data: " + ex.Message);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtProductid.Text == "") MessageBox.Show("Introduce the produdct id.");
            else
            {
                if(MessageBox.Show("Are you sure you want to delete this item?","Delete Record",MessageBoxButtons.YesNo)==DialogResult.Yes) {
                    DeleteData();
                    BindData();
                }
            }

        }
        private void SearchData()
        {
            int id = int.Parse(txtProductid.Text);
            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = "SELECT * FROM products WHERE product_id = @id";
                SqlCommand command = new SqlCommand(query, con);
                command.Parameters.AddWithValue("@id", id);

                try
                {
                    con.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvData.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchData();
        }

        private void txtProductid_TextChanged(object sender, EventArgs e)
        {

        }
    }
}