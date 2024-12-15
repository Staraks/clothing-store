using System;
using System.Data;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Npgsql;

namespace Clothing_Store
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                lblErrorMessage.Text = "Please enter both email and password.";
                lblErrorMessage.ForeColor = Color.Red;
                return;
            }

            try
            {
                // ����������� ������
                string hashedPassword = ComputeSha256Hash(password);

                // ������ � ���� ������ ��� �������� ������������
                string query = "SELECT \"role\" FROM \"user\" WHERE email = @Email AND password_hash = @PasswordHash";

                var parameters = new NpgsqlParameter[]
                {
                    new NpgsqlParameter("@Email", email),
                    new NpgsqlParameter("@PasswordHash", hashedPassword)
                };

                DataTable result = DatabaseHelper.ExecuteQuery(query, parameters);

                if (result.Rows.Count > 0)
                {
                    // ����������� �������
                    DataRow user = result.Rows[0];
                    string role = user["role"].ToString();

                    lblErrorMessage.Text = "Login successful!";
                    lblErrorMessage.ForeColor = Color.Green;

                    if (role == "admin")
                    {
                        // ������� � ��������� �����
                        AdminForm adminForm = new AdminForm();
                        adminForm.Show();
                    }
                    else
                    {
                        // ������� � ������� �����
                        MainForm mainForm = new MainForm();
                        mainForm.Show();
                    }

                    // �������� ������� �����
                    this.Hide();
                }
                else
                {
                    // ������������ email ��� ������
                    lblErrorMessage.Text = "Invalid email or password!";
                    lblErrorMessage.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "Error: " + ex.Message;
                lblErrorMessage.ForeColor = Color.Red;
            }
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            // ������� ����� �����������
            SignUpForm signUpForm = new SignUpForm();
            signUpForm.ShowDialog(); // ���������� ShowDialog, ����� ����� ��������� ��� ��������� ����
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ��������� ���
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // ����������� ����� � ������
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}


