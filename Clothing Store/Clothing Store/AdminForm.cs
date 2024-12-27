using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;

namespace Clothing_Store
{
    public partial class AdminForm : Form
    {
        public AdminForm()
        {
            InitializeComponent();
            InitializeAdminForm();
            LoadOrders();
            LoadSupplies();
        }

        private DataGridView dgvOrders;
        private DataGridView dgvSupplies;
        private ComboBox cmbOrderStatus;
        private Button btnUpdateOrderStatus;

        private TextBox txtSupplierName;
        private DateTimePicker dtpSupplyDate;
        private NumericUpDown nudDeliveryPrice;
        private ComboBox cmbProductSize;
        private ComboBox cmbProductName;
        private NumericUpDown nudQuantity;
        private Button btnSaveSupply;

        private void InitializeAdminForm()
        {
            // Настройка формы
            this.Text = "Панель администратора";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // DataGridView для заказов
            dgvOrders = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 250,
                AutoGenerateColumns = false
            };

            // Добавляем колонки для заказов
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Номер заказа", Name = "OrderId", DataPropertyName = "Номер заказа" });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Имя покупателя", Name = "CustomerName", DataPropertyName = "Имя покупателя" });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Название товара", Name = "ProductName", DataPropertyName = "Название товара" });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Размер", Name = "Size", DataPropertyName = "Размер" });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Дата заказа", Name = "OrderDate", DataPropertyName = "Дата заказа" });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Статус", Name = "Status", DataPropertyName = "Статус" });

            // ComboBox для изменения статуса заказа
            cmbOrderStatus = new ComboBox
            {
                Dock = DockStyle.Top,
                Height = 30,
                DataSource = new[] { "Ожидание", "Отправлено", "Доставлено", "Отменено" }
            };

            // Кнопка обновления статуса заказа
            btnUpdateOrderStatus = new Button
            {
                Text = "Обновить статус заказа",
                Dock = DockStyle.Top,
                Height = 40
            };
            btnUpdateOrderStatus.Click += BtnUpdateOrderStatus_Click;

            // DataGridView для поставок
            dgvSupplies = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 200,
                AutoGenerateColumns = false
            };

            // Добавляем колонки для поставок
            dgvSupplies.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Поставщик", Name = "SupplierName", DataPropertyName = "Поставщик" });
            dgvSupplies.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Дата поставки", Name = "SupplyDate", DataPropertyName = "Дата поставки" });
            dgvSupplies.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Стоимость доставки", Name = "DeliveryPrice", DataPropertyName = "Стоимость доставки" });
            dgvSupplies.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Размер товара", Name = "Size", DataPropertyName = "Размер товара" });
            dgvSupplies.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Количество", Name = "Quantity", DataPropertyName = "Количество" });

            // Поле ввода имени поставщика
            txtSupplierName = new TextBox
            {
                PlaceholderText = "Имя поставщика",
                Dock = DockStyle.Top
            };

            // Выбор даты поставки
            dtpSupplyDate = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Dock = DockStyle.Top
            };

            // Поле ввода стоимости поставки
            nudDeliveryPrice = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 100000,
                DecimalPlaces = 2,
                Dock = DockStyle.Top
            };

            // Выбор товара
            cmbProductName = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Выбор размера товара
            cmbProductSize = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Поле ввода количества
            nudQuantity = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 1000,
                Dock = DockStyle.Top
            };

            // Кнопка сохранения поставки
            btnSaveSupply = new Button
            {
                Text = "Добавить поставку",
                Dock = DockStyle.Top,
                Height = 40
            };
            btnSaveSupply.Click += BtnSaveSupply_Click;

            // Добавляем элементы на форму
            Controls.Add(btnSaveSupply);
            Controls.Add(nudQuantity);
            Controls.Add(cmbProductSize);
            Controls.Add(cmbProductName);
            Controls.Add(nudDeliveryPrice);
            Controls.Add(dtpSupplyDate);
            Controls.Add(txtSupplierName);
            Controls.Add(dgvSupplies);
            Controls.Add(btnUpdateOrderStatus);
            Controls.Add(cmbOrderStatus);
            Controls.Add(dgvOrders);

            // Подключение события для фильтрации размеров
            cmbProductName.SelectedIndexChanged += (s, e) => LoadProductSizes();

            // Загрузка данных
            LoadProductNames();
            LoadProductSizes();
        }

        private void LoadOrders()
        {
            string query = @"
                SELECT 
                    o.id AS ""Номер заказа"",
                    u.name AS ""Имя покупателя"",
                    pp.name AS ""Название товара"",
                    sz.size_name AS ""Размер"",
                    o.date AS ""Дата заказа"",
                    o.status AS ""Статус""
                FROM ""order"" o
                JOIN ""user"" u ON o.user_id = u.id
                JOIN order_details od ON o.id = od.order_id
                JOIN product_sizes ps ON od.product_size_id = ps.id
                JOIN sizes sz ON ps.size_id = sz.id
                JOIN product p ON ps.product_id = p.id
                JOIN link_product_param lpp ON p.id = lpp.product_id
                JOIN product_parameters pp ON lpp.parameters_id = pp.id";

            DataTable orders = DatabaseHelper.ExecuteQuery(query);
            dgvOrders.DataSource = orders;
        }

        private void LoadSupplies()
        {
            string query = @"
                SELECT 
                    s.supplier_name AS ""Поставщик"",
                    s.date AS ""Дата поставки"",
                    s.delivery_price AS ""Стоимость доставки"",
                    sz.size_name AS ""Размер товара"",
                    sd.quantity AS ""Количество""
                FROM supply s
                JOIN supply_details sd ON s.id = sd.supply_id
                JOIN product_sizes ps ON sd.product_size_id = ps.id
                JOIN sizes sz ON ps.size_id = sz.id";

            DataTable supplies = DatabaseHelper.ExecuteQuery(query);
            dgvSupplies.DataSource = supplies;
        }

        private void LoadProductSizes()
        {
            if (cmbProductName.SelectedValue == null)
                return;

            int productId = Convert.ToInt32(cmbProductName.SelectedValue);

            string categoryQuery = @"
                SELECT p.product_category
                FROM product p
                JOIN link_product_param lpp ON p.id = lpp.product_id
                WHERE lpp.parameters_id = @productId";
            var productCategory = DatabaseHelper.ExecuteScalar(categoryQuery, new NpgsqlParameter[] { new NpgsqlParameter("@productId", productId) })?.ToString();


            string sizeQuery = productCategory switch
            {
                "Обувь" => "SELECT id, size_name FROM sizes WHERE id BETWEEN 7 AND 16",
                "Толстовка" or "Футболка" or "Штаны" => "SELECT id, size_name FROM sizes WHERE id BETWEEN 1 AND 6",
                _ => "SELECT id, size_name FROM sizes"
            };

            DataTable sizes = DatabaseHelper.ExecuteQuery(sizeQuery);
            cmbProductSize.DataSource = sizes;
            cmbProductSize.DisplayMember = "size_name";
            cmbProductSize.ValueMember = "id";
        }

        private void LoadProductNames()
        {
            string query = "SELECT id, name FROM product_parameters";
            DataTable products = DatabaseHelper.ExecuteQuery(query);

            cmbProductName.DataSource = products;
            cmbProductName.DisplayMember = "name";
            cmbProductName.ValueMember = "id";
        }

        private void BtnUpdateOrderStatus_Click(object sender, EventArgs e)
        {
            if (dgvOrders.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите заказ для обновления статуса!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int orderId = Convert.ToInt32(dgvOrders.SelectedRows[0].Cells["OrderId"].Value);
            string status = cmbOrderStatus.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(status))
            {
                MessageBox.Show("Выберите новый статус из списка!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "UPDATE public.\"order\" SET status = @status WHERE id = @id";
            var parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("@status", status),
                new NpgsqlParameter("@id", orderId)
            };

            DatabaseHelper.ExecuteNonQuery(query, parameters);
            LoadOrders();
        }

        private void BtnSaveSupply_Click(object sender, EventArgs e)
        {
            string supplierName = txtSupplierName.Text.Trim();
            DateTime supplyDate = dtpSupplyDate.Value;
            decimal deliveryPrice = nudDeliveryPrice.Value;
            int productId = Convert.ToInt32(cmbProductName.SelectedValue);
            int sizeId = Convert.ToInt32(cmbProductSize.SelectedValue);
            int quantity = (int)nudQuantity.Value;

            if (string.IsNullOrWhiteSpace(supplierName))
            {
                MessageBox.Show("Имя поставщика обязательно!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        string insertSupplyQuery = @"
                            INSERT INTO supply (supplier_name, date, delivery_price)
                            VALUES (@supplierName, @supplyDate, @deliveryPrice)
                            RETURNING id;";
                        int supplyId;
                        using (var cmd = new NpgsqlCommand(insertSupplyQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@supplierName", supplierName);
                            cmd.Parameters.AddWithValue("@supplyDate", supplyDate);
                            cmd.Parameters.AddWithValue("@deliveryPrice", deliveryPrice);
                            supplyId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        string checkProductSizeQuery = @"
                            SELECT id FROM product_sizes
                            WHERE product_id = @productId AND size_id = @sizeId;";
                        int productSizeId;
                        using (var cmd = new NpgsqlCommand(checkProductSizeQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@productId", productId);
                            cmd.Parameters.AddWithValue("@sizeId", sizeId);
                            object result = cmd.ExecuteScalar();
                            if (result == null)
                            {
                                string insertProductSizeQuery = @"
                                    INSERT INTO product_sizes (product_id, size_id, stock_quantity)
                                    VALUES (@productId, @sizeId, 0)
                                    RETURNING id;";
                                using (var insertCmd = new NpgsqlCommand(insertProductSizeQuery, connection, transaction))
                                {
                                    insertCmd.Parameters.AddWithValue("@productId", productId);
                                    insertCmd.Parameters.AddWithValue("@sizeId", sizeId);
                                    productSizeId = Convert.ToInt32(insertCmd.ExecuteScalar());
                                }
                            }
                            else
                            {
                                productSizeId = Convert.ToInt32(result);
                            }
                        }

                        string insertSupplyDetailsQuery = @"
                            INSERT INTO supply_details (supply_id, product_size_id, quantity)
                            VALUES (@supplyId, @productSizeId, @quantity);";
                        using (var cmd = new NpgsqlCommand(insertSupplyDetailsQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@supplyId", supplyId);
                            cmd.Parameters.AddWithValue("@productSizeId", productSizeId);
                            cmd.Parameters.AddWithValue("@quantity", quantity);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }

                MessageBox.Show("Поставка успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSupplierName.Clear();
                nudDeliveryPrice.Value = 0;
                nudQuantity.Value = 1;
                LoadSupplies();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления поставки: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}











