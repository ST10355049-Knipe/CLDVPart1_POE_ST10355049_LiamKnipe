using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ST10355049.Models;
using System.Data;
using System.Data.SqlClient;
using ST10355049.Extensions;

namespace ST10355049.Controllers
{
    public class ProductDisplayController : Controller
    {
        public static string con_string = "Server=tcp:st10355049.database.windows.net,1433;Initial Catalog=st10355049-Database;Persist Security Info=False;User ID=liamknipe;Password=Lk20040119;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
        public static SqlConnection con = new SqlConnection(con_string);

        private readonly ILogger<ProductDisplayController> _logger;

        public ProductDisplayController(ILogger<ProductDisplayController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> MyWork()
        {
            try
            {
                // Fetch all products from the database
                List<productTable> products = await productTable.GetAllProductsAsync();

                // Check if products are null or empty
                if (products == null || !products.Any())
                {
                    _logger.LogWarning("No products were fetched from the database.");
                    return View("Error", "No products were fetched from the database.");
                }

                // Pass the products to the view
                return View("~/Views/Home/MyWork.cshtml", products);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while fetching products from the database.");

                // Return an error view
                return View("Error", "An error occurred while fetching products from the database.");
            }
        }

        public async Task<IActionResult> Index()
        {
            // Get all products from the database
            List<productTable> products = await productTable.GetAllProductsAsync();

            // Return the Index view with all products
            return View(products);
        }

        public IActionResult CartView()
        {
            // Fetch the cart from the session or create a new one if it doesn't exist
            Cart cart = HttpContext.Session.Get<Cart>("Cart") ?? new Cart();

            // Calculate the total amount
            decimal totalAmount = cart.TotalAmount;

            // Pass the cart to the view
            return View(cart);
        }

        public IActionResult AddToCart(int productID, int quantity)
        {
            // Check if the user is logged in
            int? userID = HttpContext.Session.GetInt32("UserID");
            if (userID == null)
            {
                // Redirect the user to the login page
                return RedirectToAction("LogIn", "Account");
            }

            // Fetch the product from the database
            string query = "SELECT * FROM productTable WHERE ProductID = @ProductID";
            using (SqlConnection con = new SqlConnection(con_string))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ProductID", productID);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Create a CartItem with the product's details
                    CartItem item = new CartItem
                    {
                        ProductID = reader.GetInt32("ProductID"),
                        Quantity = quantity,
                        Price = reader.GetDecimal("Price")
                    };

                    // Fetch the cart from the session
                    Cart cart = HttpContext.Session.Get<Cart>("Cart") ?? new Cart();

                    // Add the item to the cart
                    cart.Items.Add(item);

                    // Save the cart back to the session
                    HttpContext.Session.Set("Cart", cart);
                }
            }

            // Redirect to the cart view
            return RedirectToAction("CartView");
        }



        public IActionResult Checkout()
        {
            try
            {
                // Check if the user is logged in
                int? userID = HttpContext.Session.GetInt32("UserID");
                if (userID == null)
                {
                    // Redirect the user to the login page
                    return RedirectToAction("LogIn", "Account");
                }

                // Fetch the cart from the session
                Cart cart = HttpContext.Session.Get<Cart>("Cart");

                // Check if the cart is empty
                if (cart.Items.Count == 0)
                {
                    // Return an error message to the user
                    ViewBag.ErrorMessage = "Your cart is empty.";
                    return View();
                }

                // Create an Order object with the user's details and the current date
                orderTable order = new orderTable
                {
                    UserID = userID.Value,
                    OrderDate = DateTime.Now,
                    TotalAmount = cart.TotalAmount,
                    Status = "Pending"
                };

                // Save the order to the database
                string insertOrderQuery = "INSERT INTO orderTable (UserID, OrderDate, TotalAmount, Status) OUTPUT INSERTED.OrderID VALUES (@UserID, @OrderDate, @TotalAmount, @Status)";
                using (SqlConnection con = new SqlConnection(con_string))
                {
                    SqlCommand cmd = new SqlCommand(insertOrderQuery, con);
                    cmd.Parameters.AddWithValue("@UserID", order.UserID);
                    cmd.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                    cmd.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
                    cmd.Parameters.AddWithValue("@Status", order.Status);
                    con.Open();
                    order.OrderID = (int)cmd.ExecuteScalar();
                }

                // Check if the order was saved successfully
                if (order.OrderID == 0)
                {
                    // Return an error message to the user
                    ViewBag.ErrorMessage = "There was an error processing your order. Please try again.";
                    return View();
                }

                // Create an OrderDetails object for each item in the cart
                foreach (var item in cart.Items)
                {
                    OrderDetails orderDetails = new OrderDetails
                    {
                        OrderID = order.OrderID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        Price = item.Price
                    };

                    // Save the OrderDetails object to the database
                    string insertOrderDetailsQuery = "INSERT INTO OrderDetails (OrderID, ProductID, Quantity, Price) OUTPUT INSERTED.OrderDetailID VALUES (@OrderID, @ProductID, @Quantity, @Price)";
                    using (SqlConnection con = new SqlConnection(con_string))
                    {
                        SqlCommand cmd = new SqlCommand(insertOrderDetailsQuery, con);
                        cmd.Parameters.AddWithValue("@OrderID", orderDetails.OrderID);
                        cmd.Parameters.AddWithValue("@ProductID", orderDetails.ProductID);
                        cmd.Parameters.AddWithValue("@Quantity", orderDetails.Quantity);
                        cmd.Parameters.AddWithValue("@Price", orderDetails.Price);
                        con.Open();
                        orderDetails.OrderDetailID = (int)cmd.ExecuteScalar();
                    }

                    // Check if the OrderDetails object was saved successfully
                    if (orderDetails.OrderDetailID == 0)
                    {
                        // Return an error message to the user
                        ViewBag.ErrorMessage = "There was an error processing your order. Please try again.";
                        return View();
                    }

                    // Update the status of the product
                    string updateQuery = "UPDATE productTable SET Availability = 0 WHERE ProductID = @ProductID";
                    using (SqlConnection con = new SqlConnection(con_string))
                    {
                        SqlCommand cmd = new SqlCommand(updateQuery, con);
                        cmd.Parameters.AddWithValue("@ProductID", item.ProductID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Clear the cart
                cart.Items.Clear();
                HttpContext.Session.Set("Cart", cart);

                // Check if the cart was cleared successfully
                if (cart.Items.Count > 0)
                {
                    // Return an error message to the user
                    ViewBag.ErrorMessage = "There was an error processing your order. Please try again.";
                    return View();
                }

                // Redirect to a confirmation page
                return RedirectToAction("Confirmation");
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while processing the order.");

                // Return an error message to the user
                ViewBag.ErrorMessage = "There was an error processing your order. Please try again.";
                return View();
            }
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            // Fetch the cart from the session
            Cart cart = HttpContext.Session.Get<Cart>("Cart");

            // Find the item in the cart
            CartItem item = cart.Items.Find(i => i.ProductID == id);

            // If the item is in the cart, remove it
            if (item != null)
            {
                cart.Items.Remove(item);
            }

            // Save the cart back to the session
            HttpContext.Session.Set("Cart", cart);

            // Redirect to the cart view
            return RedirectToAction("CartView");
        }

        public IActionResult Confirmation()
        {
            try
            {
                // Check if the user is logged in
                int? userID = HttpContext.Session.GetInt32("UserID");
                if (userID == null)
                {
                    // Redirect the user to the login page
                    return RedirectToAction("LogIn", "Account");
                }

                // Fetch the order from the database
                List<orderTable> orders = orderTable.GetOrdersByUserID(userID.Value);

                // Check if the order exists
                if (orders == null || orders.Count == 0)
                {
                    return NotFound();
                }

                // Get the most recent order
                orderTable order = orders.OrderByDescending(o => o.OrderDate).First();

                // Pass the order to the view
                return View(order);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while fetching the order.");

                // Return an error message to the user
                ViewBag.ErrorMessage = "There was an error fetching your order. Please try again.";
                return View();
            }
        }
    }
}
