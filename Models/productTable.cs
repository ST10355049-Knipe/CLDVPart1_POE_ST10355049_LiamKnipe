//Liam Knipe St10355049
//REFERENCES: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/ , https://learn.microsoft.com/en-us/aspnet/web-api/overview/advanced/http-cookies
// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-5.0&tabs=visual-studio, https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-5.0
// https://www.codeproject.com/Articles/32545/Exploring-Session-in-ASP-Net , https://www.completecsharptutorial.com/asp-net-mvc5/asp-net-mvc-5-httpget-and-httppost-method-with-example.php#:~:text=HTTPGet%20method%20is%20default%20whereas,body%20of%20the%20HTTP%20request.
// https://dev.to/fabriziobagala/logging-in-aspnet-core-6-3mbh , https://stackoverflow.com/questions/11585/clearing-page-cache-in-asp-net
// https://www.c-sharpcorner.com/article/asp-net-core-3-1-authentication-and-authorization/ , https://www.youtube.com/watch?v=8_eMgS6UszY&list=PLIY8eNdw5tW_ZQawyxK0Dd1cZXwcNFWn8&ab_channel=SimpleSnippets
// https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_bootstrap.htm , https://www.guru99.com/insert-update-delete-asp-net.html#:~:text=The%20'sql'%20statement%20can%20be,are%20specified%20in%20ASP.Net.


using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace ST10355049.Models
{
    public class productTable
    {
        public static string con_string = "Server=tcp:st10355049.database.windows.net,1433;Initial Catalog=st10355049-Database;Persist Security Info=False;User ID=liamknipe;Password=Lk20040119;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
        public static SqlConnection con = new SqlConnection(con_string);


        public int ProductID { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        public string Category { get; set; } // Nullable

        [Required(ErrorMessage = "Availability is required.")]
        public bool Availability { get; set; }

        public string Description { get; set; } // Nullable

        [Required(ErrorMessage = "Image URL is required.")]
        public string ImageUrl { get; set; }

        public async Task<int> InsertProductAsync()
        {
            string insertQuery = @"INSERT INTO productTable 
                           (ProductName, Price, Category, Availability, Description, ImageUrl) 
                           VALUES (@ProductName, @Price, @Category, @Availability, @Description, @ImageUrl)"; // Changed from Name to ProductName

            using (SqlConnection conn = new SqlConnection(con_string))
            using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
            {
                cmd.Parameters.AddWithValue("@ProductName", ProductName); // Changed from Name to ProductName
                cmd.Parameters.AddWithValue("@Price", Price);
                cmd.Parameters.AddWithValue("@Category", Category ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Availability", Availability);
                cmd.Parameters.AddWithValue("@Description", Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ImageUrl", ImageUrl);

                conn.Open();
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected;
            }
        }


        public async static Task<List<productTable>> GetAllProductsAsync()
        {
            List<productTable> products = new List<productTable>();
            try
            {
                using (SqlConnection con = new SqlConnection(con_string))
                {
                    string sql = "SELECT * FROM productTable";
                    SqlCommand cmd = new SqlCommand(sql, con);

                    await con.OpenAsync();
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                    while (await rdr.ReadAsync())
                    {
                        productTable product = new productTable();
                        product.ProductID = Convert.ToInt32(rdr["ProductID"]);
                        product.ProductName = rdr["ProductName"].ToString();
                        product.Price = Convert.ToDecimal(rdr["Price"]);
                        product.Category = rdr["Category"].ToString();
                        product.Availability = Convert.ToBoolean(rdr["Availability"]);
                        product.Description = rdr["Description"].ToString();
                        product.ImageUrl = await rdr.GetFieldValueAsync<string>("ImageUrl");

                        products.Add(product);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine($"Fetched {products.Count} products from the database.");
            return products;
        }
    }
}