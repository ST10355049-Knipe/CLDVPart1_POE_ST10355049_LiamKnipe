using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ST10355049.Models
{
    public class productTable 
    {
        public static string con_string = "Server=tcp:st10355049.database.windows.net,1433;Initial Catalog=st10355049-Database;Persist Security Info=False;User ID=liamknipe;Password=Lk20040119;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
        public static SqlConnection con = new SqlConnection(con_string);

        public int ProductID { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; }

        public bool Availability { get; set; }

        public string Description { get; set; }

        public async Task<int> InsertProductAsync(productTable product)
        {
            try
            {
                string sql = "INSERT INTO productTable (Name, Price, Category, Availability, Description) VALUES (@Name, @Price, @Category, @Availability, @Description)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Name", product.Name);
                cmd.Parameters.AddWithValue("@Price", product.Price);
                cmd.Parameters.AddWithValue("@Category", product.Category);
                cmd.Parameters.AddWithValue("@Availability", product.Availability);
                cmd.Parameters.AddWithValue("@Description", product.Description ?? (object)DBNull.Value);

                await con.OpenAsync();
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                con.Close();

                return rowsAffected;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async static Task<List<productTable>> GetAllProductsAsync()
        {
            List<productTable> products = new List<productTable>();
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
                    product.Name = rdr["Name"].ToString();
                    product.Price = Convert.ToDecimal(rdr["Price"]);
                    product.Category = rdr["Category"].ToString();
                    product.Availability = Convert.ToBoolean(rdr["Availability"]);
                    product.Description = rdr["Description"].ToString();

                    products.Add(product);
                }
            }

            return products;
        }
    