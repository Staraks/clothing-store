using System.Collections.Generic;

namespace Clothing_Store
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public static class CartManager
    {
        private static List<CartItem> cartItems = new List<CartItem>();

        // Добавление товара в корзину
        public static void AddToCart(int productId, string name, string size, decimal price)
        {
            var existingItem = cartItems.Find(item => item.ProductId == productId && item.Size == size);

            if (existingItem != null)
            {
                existingItem.Quantity++; 
            }
            else
            {
                cartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Name = name,
                    Size = size,
                    Price = price,
                    Quantity = 1
                });
            }
        }

        // Получение всех товаров в корзине
        public static List<CartItem> GetCartItems()
        {
            return cartItems;
        }

        // Удаление товара из корзины
        public static void RemoveFromCart(CartItem item)
        {
            cartItems.Remove(item);
        }

        // Очистка корзины
        public static void ClearCart()
        {
            cartItems.Clear();
        }

        // Общая сумма корзины
        public static decimal GetTotalAmount()
        {
            decimal total = 0;
            foreach (var item in cartItems)
            {
                total += item.Price * item.Quantity;
            }
            return total;
        }
    }
}
