using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Npgsql;

namespace Clothing_Store
{
    public partial class SignUpForm : Form
    {
        public SignUpForm()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {

            // Проверка полей формы
            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phoneNumber = txtPhoneNumber.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Все поля должны быть заполнены.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Регистрация пользователя
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    // Проверка, существует ли email
                    string checkQuery = "SELECT COUNT(*) FROM \"user\" WHERE email = @Email";
                    using (var command = new NpgsqlCommand(checkQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        int userExists = Convert.ToInt32(command.ExecuteScalar());

                        if (userExists > 0)
                        {
                            MessageBox.Show("Пользователь с таким адресом электронной почты уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Хэширование пароля
                    string hashedPassword = ComputeHash(password);

                    // Вставка пользователя
                    string insertQuery = "INSERT INTO \"user\" (name, email, phone_number, password_hash, role) VALUES (@Name, @Email, @PhoneNumber, @PasswordHash, @Role)";
                    using (var command = new NpgsqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        command.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                        command.Parameters.AddWithValue("@Role", "Customer");

                        command.ExecuteNonQuery();
                        MessageBox.Show("Регистрация прошла успешно!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close(); 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка во время регистрации: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ComputeHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}


