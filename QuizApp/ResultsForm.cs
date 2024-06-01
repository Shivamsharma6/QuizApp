using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuizApp
{
    public partial class ResultsForm : Form
    {
        private int userId;
        private string connectionString = "Server=localhost;Database=QuizDB;Trusted_Connection=True";

        public ResultsForm(int userId)
        {
            this.userId = userId;
            InitializeComponent();
            LoadResults();
        }

        private void LoadResults()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Score, Timestamp FROM QuizResults WHERE UserID = @UserID ORDER BY Timestamp DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable resultsTable = new DataTable();
                    adapter.Fill(resultsTable);

                    dataGridViewResults.DataSource = resultsTable;
                }
            }
        }

        // Handle FormClosed event to show MainForm when ResultsForm is closed
        private void ResultsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1 mainForm = new Form1(userId);
            mainForm.Show();
        }
    }
}
