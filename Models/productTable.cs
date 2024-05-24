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