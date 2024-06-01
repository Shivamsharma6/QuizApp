using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuizApp
{
    public partial class UserForm : Form
    {
        private string connectionString = "Server=localhost;Database=QuizDB;Trusted_Connection=True";

        public UserForm()
        {
            InitializeComponent();
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string email = txtEmail.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            int userId = VerifyUser(username, email);

            if (userId > 0)
            {
                var result = MessageBox.Show("User found. Do you want to view previous quiz results?", "User Found", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Yes)
                {
                    // Show previous quiz results
                    ShowPreviousQuizResults(userId);
                }
                else if (result == DialogResult.No)
                {
                    // Start a new quiz
                    Form1 mainForm = new Form1(userId);
                    mainForm.Show();
                    this.Hide();
                }
                // If Cancel, do nothing
            }
            else
            {
                MessageBox.Show("User not found. Please register.");
            }
        }

        private int VerifyUser(string username, string email)
        {
            int userId = -1;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT UserID FROM Users WHERE Username = @Username AND Email = @Email";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Email", email);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        userId = Convert.ToInt32(result);
                    }
                }
            }

            return userId;
        }

        private void ShowPreviousQuizResults(int userId)
        {
            // Fetch and display previous quiz results for the user
            ResultsForm resultsForm = new ResultsForm(userId);
            resultsForm.Show();
            this.Hide();
            // Code to fetch and display quiz results
        }
    }
}
