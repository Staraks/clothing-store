using Npgsql;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Clothing_Store
{
    public partial class CartForm : Form
    {
        private int _userId;
        public CartForm(int userId)
        {
            InitializeComponent();
            LoadCartItems();
            _userId = userId;
        }

        private void LoadCartItems()
        {
            listViewCart.Items.Clear();

            var cartItems = CartManager.GetCartItems();
            foreach (var item in cartItems)
            {
                var listItem = new ListViewItem(item.Name);
                listItem.SubItems.Add(item.Size);
                listItem.SubItems.Add(item.Quantity.ToString());
                listItem.SubItems.Add(item.Price.ToString("F2"));
                listItem.SubItems.Add((item.Price * item.Quantity).ToString("F2"));

                listViewCart.Items.Add(listItem);
            }

            lblTotal.Text = $"Итого: {CartManager.GetTotalAmount():F2}";
        }

        private void BtnPlaceOrder_Click(object sender, EventArgs e)
        {
            var cartItems = CartManager.GetCartItems();
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Ваша корзина пуста. Добавьте товары перед оформлением заказа.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Получение адреса доставки из текстового поля
            string shippingAddress = txtShippingAddress.Text.Trim();
            if (string.IsNullOrWhiteSpace(shippingAddress))
            {
                MessageBox.Show("Пожалуйста введите адрес доставки.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        // Вставляем новый заказ в таблицу "order"
                        string insertOrderQuery = @"
                INSERT INTO ""order"" (user_id, date, total_amount, status, shipping_address)
                VALUES (@userId, NOW(), @totalAmount, 'Ожидание', @shippingAddress)
                RETURNING id;";

                        int orderId;
                        using (var cmd = new NpgsqlCommand(insertOrderQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@userId", _userId);
                            cmd.Parameters.AddWithValue("@totalAmount", CartManager.GetTotalAmount());
                            cmd.Parameters.AddWithValue("@shippingAddress", shippingAddress);

                            orderId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // SQL-запросы для деталей заказа
                        string insertOrderDetailsQuery = @"
                INSERT INTO order_details (order_id, product_size_id, price, quantity)
                VALUES (@orderId, @productSizeId, @price, @quantity);";

                        string updateStockQuery = @"
                UPDATE product_sizes
                SET stock_quantity = stock_quantity - @quantity
                WHERE id = @productSizeId
                  AND stock_quantity >= @quantity;"; 

                        foreach (var item in cartItems)
                        {
                            // Получаем product_size_id для текущего товара и размера
                            string getProductSizeIdQuery = @"
                    SELECT ps.id
                    FROM product_sizes ps
                    JOIN sizes s ON ps.size_id = s.id
                    WHERE ps.product_id = @productId AND s.size_name = @sizeName;";

                            int productSizeId;
                            using (var cmdGetProductSizeId = new NpgsqlCommand(getProductSizeIdQuery, connection, transaction))
                            {
                                cmdGetProductSizeId.Parameters.AddWithValue("@productId", item.ProductId);
                                cmdGetProductSizeId.Parameters.AddWithValue("@sizeName", item.Size);

                                var result = cmdGetProductSizeId.ExecuteScalar();
                                if (result == null)
                                {
                                    throw new Exception($"Размер продукта не найден {item.Name}, size {item.Size}.");
                                }

                                productSizeId = Convert.ToInt32(result);
                            }

                            // Вставляем данные в order_details
                            using (var cmdInsert = new NpgsqlCommand(insertOrderDetailsQuery, connection, transaction))
                            {
                                cmdInsert.Parameters.AddWithValue("@orderId", orderId);
                                cmdInsert.Parameters.AddWithValue("@productSizeId", productSizeId);
                                cmdInsert.Parameters.AddWithValue("@price", item.Price);
                                cmdInsert.Parameters.AddWithValue("@quantity", item.Quantity);

                                cmdInsert.ExecuteNonQuery();
                            }

                            // Обновляем остаток товаров в product_sizes
                            using (var cmdUpdateStock = new NpgsqlCommand(updateStockQuery, connection, transaction))
                            {
                                cmdUpdateStock.Parameters.AddWithValue("@productSizeId", productSizeId);
                                cmdUpdateStock.Parameters.AddWithValue("@quantity", item.Quantity);

                                int rowsAffected = cmdUpdateStock.ExecuteNonQuery();

                                if (rowsAffected == 0)
                                {
                                    throw new Exception($"Недостаточно товара на складе {item.Name}, size {item.Size}.");
                                }
                            }
                        }

                        // Подтверждение транзакции
                        transaction.Commit();

                        MessageBox.Show("Ваш заказ успешно оформлен!", "Success",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                        CartManager.ClearCart(); 
                        LoadCartItems(); 
                        txtShippingAddress.Clear(); 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка оформления заказа: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnClearCart_Click(object sender, EventArgs e)
        {
            CartManager.ClearCart();
            LoadCartItems();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

