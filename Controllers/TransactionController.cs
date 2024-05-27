//Liam Knipe St10355049
//REFERENCES: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/ , https://learn.microsoft.com/en-us/aspnet/web-api/overview/advanced/http-cookies
// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-5.0&tabs=visual-studio, https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-5.0
// https://www.codeproject.com/Articles/32545/Exploring-Session-in-ASP-Net , https://www.completecsharptutorial.com/asp-net-mvc5/asp-net-mvc-5-httpget-and-httppost-method-with-example.php#:~:text=HTTPGet%20method%20is%20default%20whereas,body%20of%20the%20HTTP%20request.
// https://dev.to/fabriziobagala/logging-in-aspnet-core-6-3mbh , https://stackoverflow.com/questions/11585/clearing-page-cache-in-asp-net
// https://www.c-sharpcorner.com/article/asp-net-core-3-1-authentication-and-authorization/ , https://www.youtube.com/watch?v=8_eMgS6UszY&list=PLIY8eNdw5tW_ZQawyxK0Dd1cZXwcNFWn8&ab_channel=SimpleSnippets
// https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_bootstrap.htm , https://www.guru99.com/insert-update-delete-asp-net.html#:~:text=The%20'sql'%20statement%20can%20be,are%20specified%20in%20ASP.Net.


using Microsoft.AspNetCore.Mvc;
using ST10355049.Models;
using System.Data.SqlClient;

namespace ST10355049.Controllers
{
    public class TransactionController : Controller
    {
        [HttpPost]
        public ActionResult PlaceOrder(int userID, int productID)
        {
            try
            {
                // Create a new instance of SqlConnection using the connection string
                using (SqlConnection con = new SqlConnection(productTable.con_string))
                {
                    // Define the SQL query to insert a new record into the transactionTable
                    string sql = "INSERT INTO transactionTable (userID, productID) VALUES (@UserID, @ProductID)";

                    // Create a new instance of SqlCommand with the SQL query and SqlConnection
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        // Add parameters to the SqlCommand for userID and productID
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@ProductID", productID);

                        // Open the SqlConnection
                        con.Open();

                        // Execute the SqlCommand to insert the record into the transactionTable
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Close the SqlConnection
                        con.Close();

                        // Check if the insert operation was successful
                        if (rowsAffected > 0)
                        {
                            // Redirect the user to the home page after placing the order
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            // If the insert operation failed, return an error view or message
                            return View("OrderFailed");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For now, return an error view or message
                return View("Error");
            }
        }
    }
}