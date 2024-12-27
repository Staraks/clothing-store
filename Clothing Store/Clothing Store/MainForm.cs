using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;

namespace Clothing_Store
{
    public partial class MainForm : Form
    {
        private Button btnLogin;
        private Button btnSignUp;
        private Button btnViewCart;
        private Panel headerPanel; // Панель для кнопок
        private FlowLayoutPanel flowLayoutPanelProducts; // Каталог товаров
        private ComboBox cbCategoryFilter; // Фильтр по категориям
        private int? _userId = null;

        public MainForm()
        {
            InitializeHeaderPanel();
            InitializeProductsPanel();
            LoadProducts(); // Загружаем товары при запуске формы
        }

        private void InitializeHeaderPanel()
        {
            // Панель вверху формы для кнопок
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.LightGray
            };

            // Кнопка "Login"
            btnLogin = new Button
            {
                Text = "Вход",
                Width = 100,
                Height = 30,
                Left = 10,
                Top = 10
            };
            btnLogin.Click += BtnLogin_Click;

            // Кнопка "Sign Up"
            btnSignUp = new Button
            {
                Text = "Регистрация",
                Width = 105,
                Height = 30,
                Left = 120,
                Top = 10
            };
            btnSignUp.Click += BtnSignUp_Click;

            // Кнопка "Просмотр корзины"
            btnViewCart = new Button
            {
                Text = "Корзина",
                Width = 100,
                Height = 30,
                Left = 230,
                Top = 10
            };
            btnViewCart.Click += BtnViewCart_Click;

            // Фильтр по категориям
            cbCategoryFilter = new ComboBox
            {
                Width = 150,
                Height = 30,
                Left = 350,
                Top = 10
            };
            cbCategoryFilter.SelectedIndexChanged += CbCategoryFilter_SelectedIndexChanged;

            // Загружаем категории
            LoadCategories();

            // Добавление элементов на панель
            headerPanel.Controls.Add(btnLogin);
            headerPanel.Controls.Add(btnSignUp);
            headerPanel.Controls.Add(btnViewCart);
            headerPanel.Controls.Add(cbCategoryFilter);

            // Добавляем панель на форму
            this.Controls.Add(headerPanel);
        }

        private void InitializeProductsPanel()
        {
            flowLayoutPanelProducts = new FlowLayoutPanel
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                Width = this.ClientSize.Width,
                Height = this.ClientSize.Height - headerPanel.Height, // Учитываем высоту headerPanel
                Top = headerPanel.Bottom,
                Left = 0,
                AutoScroll = true,
                BackColor = Color.White,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(20)
            };

            this.Controls.Add(flowLayoutPanelProducts);

            // Увеличение размеров формы
            this.Text = "Каталог товаров";
            this.Size = new Size(1100, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            var loginForm = new LoginForm();
            loginForm.FormClosed += (o, args) =>
            {
                if (loginForm.UserId.HasValue)
                {
                    _userId = loginForm.UserId;
                    MessageBox.Show("Добро пожаловать!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            loginForm.ShowDialog();
        }

        private void BtnSignUp_Click(object sender, EventArgs e)
        {
            var signUpForm = new SignUpForm();
            signUpForm.ShowDialog();
        }

        private void BtnViewCart_Click(object sender, EventArgs e)
        {
            if (!_userId.HasValue)
            {
                MessageBox.Show("Пожалуйста, авторизуйтесь чтобы открыть корзину.", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var cartForm = new CartForm(_userId.Value);
            cartForm.ShowDialog();
        }

        private void LoadCategories()
        {
            try
            {
                cbCategoryFilter.SelectedIndexChanged -= CbCategoryFilter_SelectedIndexChanged; // Отключаем обработчик

                string query = "SELECT DISTINCT product_category FROM product";

                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        cbCategoryFilter.Items.Add("Все категории"); // Вариант без фильтра
                        while (reader.Read())
                        {
                            string category = reader["product_category"].ToString();
                            cbCategoryFilter.Items.Add(category);
                        }
                    }
                }

                cbCategoryFilter.SelectedIndex = 0; // Устанавливаем категорию по умолчанию
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки категорий: " + ex.Message);
            }
            finally
            {
                cbCategoryFilter.SelectedIndexChanged += CbCategoryFilter_SelectedIndexChanged; // Включаем обработчик
            }
        }


        private void LoadProducts(string selectedCategory = null)
        {
            try
            {
                // Базовый SQL-запрос
                string query = @"
                    SELECT p.id, pp.name, pp.color, pp.image_path, p.price
                    FROM product p
                    JOIN link_product_param lpp ON p.id = lpp.product_id
                    JOIN product_parameters pp ON lpp.parameters_id = pp.id";

                // Если выбрана категория, добавляем фильтр
                if (!string.IsNullOrEmpty(selectedCategory) && selectedCategory != "Все категории")
                {
                    query += " WHERE p.product_category = @category";
                }

                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(selectedCategory) && selectedCategory != "Все категории")
                        {
                            command.Parameters.AddWithValue("@category", selectedCategory);
                        }

                        using (var reader = command.ExecuteReader())
                        {
                            flowLayoutPanelProducts.Controls.Clear(); // Очищаем каталог

                            while (reader.Read())
                            {
                                int productId = Convert.ToInt32(reader["id"]);
                                string name = reader["name"].ToString();
                                decimal price = Convert.ToDecimal(reader["price"]);
                                string imagePath = reader["image_path"].ToString();

                                var productCard = CreateProductCard(productId, name, price, imagePath);
                                flowLayoutPanelProducts.Controls.Add(productCard);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки товаров: " + ex.Message);
            }
        }

        private void CbCategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCategory = cbCategoryFilter.SelectedItem?.ToString();
            LoadProducts(selectedCategory); // Загружаем товары с фильтром по категории
        }

        private Panel CreateProductCard(int productId, string name, decimal price, string imagePath)
        {
            Panel panel = new Panel
            {
                Width = 300,
                Height = 400,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(20),
                BackColor = Color.White
            };

            PictureBox pictureBox = new PictureBox
            {
                Width = 280,
                Height = 200,
                Top = 10,
                Left = 10,
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = LoadImage(imagePath)
            };

            Label lblName = new Label
            {
                Text = name.Length > 30 ? name.Substring(0, 30) + "..." : name,
                AutoSize = false,
                Width = 280,
                Height = 40,
                Top = 220,
                Left = 10,
                Font = new Font("Arial", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.TopLeft
            };

            Label lblPrice = new Label
            {
                Text = $"Цена: {price:F2}",
                AutoSize = true,
                Top = lblName.Bottom + 10,
                Left = 10,
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.Green
            };

            ComboBox cbSizes = CreateSizeComboBox(productId);
            cbSizes.Top = lblPrice.Bottom + 10;
            cbSizes.Left = 10;
            cbSizes.Width = 280;

            Button btnAddToCart = new Button
            {
                Text = "Добавить в корзину",
                Width = 280,
                Height = 30,
                Top = cbSizes.Bottom + 10,
                Left = 10
            };

            btnAddToCart.Click += (sender, e) =>
            {
                if (!_userId.HasValue)
                {
                    MessageBox.Show("Пожалуйста, авторизуйтесь чтобы добавить товар в корзину.", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedSize = cbSizes.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedSize))
                {
                    MessageBox.Show("Выберите размер.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (CheckStock(productId, selectedSize))
                {
                    CartManager.AddToCart(productId, name, selectedSize, price);
                    MessageBox.Show($"{name} (Размер: {selectedSize}) добавлен в корзину.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Выбранного размера нет в наличии.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            panel.Controls.Add(pictureBox);
            panel.Controls.Add(lblName);
            panel.Controls.Add(lblPrice);
            panel.Controls.Add(cbSizes);
            panel.Controls.Add(btnAddToCart);

            return panel;
        }

        private ComboBox CreateSizeComboBox(int productId)
        {
            ComboBox cbSizes = new ComboBox();
            try
            {
                string query = @"
                    SELECT s.size_name 
                    FROM product_sizes ps
                    JOIN sizes s ON ps.size_id = s.id
                    WHERE ps.product_id = @productId AND ps.stock_quantity > 0";

                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@productId", productId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cbSizes.Items.Add(reader["size_name"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки размеров: " + ex.Message);
            }

            return cbSizes;
        }

        private bool CheckStock(int productId, string size)
        {
            try
            {
                string query = @"
                    SELECT ps.stock_quantity
                    FROM product_sizes ps
                    JOIN sizes s ON ps.size_id = s.id
                    WHERE ps.product_id = @productId AND s.size_name = @sizeName";

                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@productId", productId);
                        command.Parameters.AddWithValue("@sizeName", size);

                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            int stockQuantity = Convert.ToInt32(result);
                            return stockQuantity > 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking stock: " + ex.Message);
            }

            return false;
        }

        private Image LoadImage(string imagePath)
        {
            try
            {
                string fullPath = System.IO.Path.Combine(Application.StartupPath, imagePath);
                if (System.IO.File.Exists(fullPath))
                    return Image.FromFile(fullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки изображения: {ex.Message}");
            }

            return null;
        }
    }
}


















