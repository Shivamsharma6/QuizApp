using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuizApp
{
    public partial class Form1 : Form
    {
        private int userId;
        private int currentQuestionIndex;
        private int score;
        private DataTable questionsTable;
        private string connectionString = "Server=localhost;Database=QuizDB;Trusted_Connection=True;";
        public Form1(int userId)
        {
            this.userId = userId;
            InitializeComponent();
            SetupQuiz();
        }

        private void SetupQuiz()
        {
            LoadQuestionsFromDatabase();
            LoadQuestion();
        }

        private void LoadQuestionsFromDatabase()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT q.QuestionID, q.QuestionText, o.OptionID, o.OptionText, o.IsCorrect
                    FROM Questions q
                    JOIN Options o ON q.QuestionID = o.QuestionID
                    ORDER BY q.QuestionID, o.OptionID";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                questionsTable = ds.Tables[0];
            }
        }

        private void LoadQuestion()
        {
            if (currentQuestionIndex < questionsTable.Rows.Count / 4)
            {
                var currentQuestionRows = questionsTable.Select($"QuestionID = {currentQuestionIndex + 1}");

                lblQuestion.Text = currentQuestionRows[0]["QuestionText"].ToString();
                rdoOption1.Text = currentQuestionRows[0]["OptionText"].ToString();
                rdoOption2.Text = currentQuestionRows[1]["OptionText"].ToString();
                rdoOption3.Text = currentQuestionRows[2]["OptionText"].ToString();
                rdoOption4.Text = currentQuestionRows[3]["OptionText"].ToString();

                rdoOption1.Tag = currentQuestionRows[0]["IsCorrect"];
                rdoOption2.Tag = currentQuestionRows[1]["IsCorrect"];
                rdoOption3.Tag = currentQuestionRows[2]["IsCorrect"];
                rdoOption4.Tag = currentQuestionRows[3]["IsCorrect"];

                rdoOption1.Checked = false;
                rdoOption2.Checked = false;
                rdoOption3.Checked = false;
                rdoOption4.Checked = false;
            }
            else
            {
                SaveQuizResult();
                MessageBox.Show($"Quiz Over! Your score is {score}/{questionsTable.Rows.Count / 4}");
                ShowRetakeOrNewUserOption();
            }
        }

        private void SaveQuizResult()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO QuizResults (UserID, Score, Timestamp) VALUES (@UserID, @Score, @Timestamp)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Score", score);
                    cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void ShowRetakeOrNewUserOption()
        {
            var result = MessageBox.Show("Do you want to retake the quiz as the same user?", "Retake Quiz", MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Yes)
            {
                score = 0;
                currentQuestionIndex = 0;
                LoadQuestion();
            }
            else if (result == DialogResult.No)
            {
                UserForm userForm = new UserForm();
                userForm.Show();
                this.Hide();
            }
            else
            {
                Application.Exit();
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            RadioButton[] options = { rdoOption1, rdoOption2, rdoOption3, rdoOption4 };
            foreach (var option in options)
            {
                if (option.Checked && (bool)option.Tag)
                {
                    score++;
                    lblScore.Text = $"Score: {score}";
                }
            }

            currentQuestionIndex++;
            progressBar.Value = (currentQuestionIndex * 100) / (questionsTable.Rows.Count / 4);
            LoadQuestion();
        }
    }
}
