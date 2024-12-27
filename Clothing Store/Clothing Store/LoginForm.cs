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
        public int? UserId { get; private set; }

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
                lblErrorMessage.Text = "Пожалуйста, введите адрес электронной почты и пароль.";
                lblErrorMessage.ForeColor = Color.Red;
                return;
            }

            try
            {
                string hashedPassword = ComputeSha256Hash(password);

                string query = "SELECT id, role FROM \"user\" WHERE email = @Email AND password_hash = @PasswordHash";

                var parameters = new NpgsqlParameter[]
                {
                    new NpgsqlParameter("@Email", email),
                    new NpgsqlParameter("@PasswordHash", hashedPassword)
                };

                DataTable result = DatabaseHelper.ExecuteQuery(query, parameters);

                if (result.Rows.Count > 0)
                {
                    UserId = Convert.ToInt32(result.Rows[0]["id"]);
                    string role = result.Rows[0]["role"].ToString();

                    if (role == "admin")
                    {
                        // Открываем AdminForm
                        AdminForm adminForm = new AdminForm();
                        this.Hide();
                        adminForm.ShowDialog();
                        this.Show();
                    }
                    else
                    {
                        // Обычный пользователь
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {
                    lblErrorMessage.Text = "Неверный адрес электронной почты или пароль!";
                    lblErrorMessage.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "Ошибка: " + ex.Message;
                lblErrorMessage.ForeColor = Color.Red;
            }
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
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




