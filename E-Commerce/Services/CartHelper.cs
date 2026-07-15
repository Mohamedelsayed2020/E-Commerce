using Azure.Core;
using E_Commerce.Models;
using System.Text.Json;

namespace E_Commerce.Services
{
    public class CartHelper
    {
        public static Dictionary<int, int> GetCartDictionary(HttpRequest request, HttpResponse response)
        {
            string cookieValue = request.Cookies["shopping_cart"] ?? "";
            try
            {
                //var cart = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(cookieValue));
                var cart = Uri.UnescapeDataString(cookieValue);
                var dictionary = JsonSerializer.Deserialize<Dictionary<int, int>>(cart);
               
                if (dictionary != null)
                {
                    return dictionary;
                }
            }
            catch (Exception ex)
            {
                // Log the error instead of throwing
                System.Diagnostics.Debug.WriteLine($"Error parsing cart cookie: {ex.Message}");

                // Delete the corrupted cookie
                response.Cookies.Delete("shopping_cart");
            }

            if (cookieValue.Length > 0)
            {
                response.Cookies.Delete("shopping_cart");
            }
            return new Dictionary<int, int>();
        }

        public static int GetCartSize(HttpRequest request, HttpResponse response)
        {
            int cartSize = 0;
            var cartDictionary = GetCartDictionary(request, response);
            foreach (var item in cartDictionary)
            {
                cartSize += item.Value;
            }
            return cartSize;
        }

        public static List<OrderItem> GetCartItems(HttpRequest request, HttpResponse response, ApplicarionDbContext context)
        {
            var cartItems = new List<OrderItem>();
            var dictionary = GetCartDictionary(request, response);
            foreach (var item in dictionary)
            {
                int productId = item.Key;

                int Quantity = item.Value;
                var product = context.Products.FirstOrDefault(p => p.Id == item.Key);
                if (product != null)
                {
                    var orderItem = new OrderItem
                    {
                        Product = product,
                        Quantity = Quantity,
                        UnitPrice = product.Price
                    };
                    cartItems.Add(orderItem);
                }

            }

            return cartItems;
        }

        public static decimal GetSubTotal(List<OrderItem> cartItems)
        {
            decimal subTotal = 0;
            foreach (var item in cartItems)
            {
                subTotal += item.UnitPrice * item.Quantity;
            }
            return subTotal;
        }
    }
}
