using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Npgsql;

namespace Clothing_Store
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            LoadProducts(); // Загрузка товаров при запуске формы
        }

        private void LoadProducts()
        {
            try
            {
                // Подключение и SQL-запрос
                string query = @"
                    SELECT p.id, pp.name, pp.size, pp.color, p.price, pp.image_path 
                    FROM product p
                    JOIN link_product_param lpp ON p.id = lpp.product_id
                    JOIN product_parameters pp ON lpp.parameters_id = pp.id";

                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Считываем данные
                            string name = reader["name"].ToString();
                            string size = reader["size"].ToString();
                            string color = reader["color"].ToString();
                            decimal price = Convert.ToDecimal(reader["price"]);
                            string imagePath = reader["image_path"].ToString();

                            // Создаём карточку для товара
                            var productCard = CreateProductCard(name, size, color, price, imagePath);

                            // Добавляем карточку на FlowLayoutPanel
                            flowLayoutPanelProducts.Controls.Add(productCard);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message);
            }
        }

        private Panel CreateProductCard(string name, string size, string color, decimal price, string imagePath)
        {
            // Создаём панель для карточки
            Panel panel = new Panel
            {
                Width = 200,
                Height = 300,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(10),
                BackColor = Color.White
            };

            // Добавляем изображение
            PictureBox pictureBox = new PictureBox
            {
                Width = 180,
                Height = 180,
                Top = 10,
                Left = 10,
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = LoadImage(imagePath)
            };

            // Добавляем название товара
            Label lblName = new Label
            {
                Text = name,
                AutoSize = false,       // Отключаем авторазмер
                Width = 180,            // Ширина карточки
                Height = 40,            // Высота названия
                Top = 200,              // Отступ сверху
                Left = 10,              // Отступ слева
                Font = new Font("Arial", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter // Центрирование текста
            };


            // Добавляем описание (только размер)
            Label lblDetails = new Label
            {
                Text = $"Size: {size}",
                AutoSize = true,
                Top = 240,
                Left = 10,
                Font = new Font("Arial", 9)
            };

            // Добавляем цену
            Label lblPrice = new Label
            {
                Text = $"Price: {price:F2}",
                AutoSize = true,
                Top = 250,
                Left = 10,
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.Green
            };

            // Добавляем кнопку "Добавить в корзину"
            Button btnAddToCart = new Button
            {
                Text = "Add to Cart",
                Width = 180,
                Height = 30,
                Top = 270,
                Left = 10
            };

            btnAddToCart.Click += (sender, e) => AddToCart(name);

            // Добавляем элементы на панель
            panel.Controls.Add(pictureBox);
            panel.Controls.Add(lblName);
            panel.Controls.Add(lblDetails);
            panel.Controls.Add(lblPrice);
            panel.Controls.Add(btnAddToCart);

            return panel;
        }

        private Image LoadImage(string imagePath)
        {
            try
            {
                string fullPath = Path.Combine(Application.StartupPath, imagePath);
                if (File.Exists(fullPath))
                    return Image.FromFile(fullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading image: {ex.Message}");
            }

            // Если изображение не найдено или возникла ошибка, возвращаем null
            return null;
        }




        private void AddToCart(string productName)
        {
            MessageBox.Show($"{productName} has been added to your cart.", "Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

