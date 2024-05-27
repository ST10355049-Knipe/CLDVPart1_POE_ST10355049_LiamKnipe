//Liam Knipe St10355049
//REFERENCES: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/ , https://learn.microsoft.com/en-us/aspnet/web-api/overview/advanced/http-cookies
// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-5.0&tabs=visual-studio, https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-5.0
// https://www.codeproject.com/Articles/32545/Exploring-Session-in-ASP-Net , https://www.completecsharptutorial.com/asp-net-mvc5/asp-net-mvc-5-httpget-and-httppost-method-with-example.php#:~:text=HTTPGet%20method%20is%20default%20whereas,body%20of%20the%20HTTP%20request.
// https://dev.to/fabriziobagala/logging-in-aspnet-core-6-3mbh , https://stackoverflow.com/questions/11585/clearing-page-cache-in-asp-net
// https://www.c-sharpcorner.com/article/asp-net-core-3-1-authentication-and-authorization/ , https://www.youtube.com/watch?v=8_eMgS6UszY&list=PLIY8eNdw5tW_ZQawyxK0Dd1cZXwcNFWn8&ab_channel=SimpleSnippets
// https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_bootstrap.htm , https://www.guru99.com/insert-update-delete-asp-net.html#:~:text=The%20'sql'%20statement%20can%20be,are%20specified%20in%20ASP.Net.


using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace ST10355049.Models
{
    public class userTable
    {
        public static string ConnectionString = "Server=tcp:st10355049.database.windows.net,1433;Initial Catalog=st10355049-Database;Persist Security Info=False;User ID=liamknipe;Password=Lk20040119;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";

        public static SqlConnection con = new SqlConnection(ConnectionString);

        public int UserID { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        public async Task<int> InsertUserAsync()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    string sql = "INSERT INTO userTable (FirstName, LastName, Email, Password) VALUES (@FirstName, @LastName, @Email, @Password)";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@FirstName", this.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", this.LastName);
                    cmd.Parameters.AddWithValue("@Email", this.Email);
                    cmd.Parameters.AddWithValue("@Password", HashPassword(this.Password));

                    await con.OpenAsync();
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    con.Close();

                    return rowsAffected;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        private static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public async Task<int> UpdateUserAsync()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                string sql = "UPDATE userTable SET FirstName = @FirstName, LastName = @LastName, Email = @Email WHERE UserID = @UserID";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@FirstName", this.FirstName);
                cmd.Parameters.AddWithValue("@LastName", this.LastName);
                cmd.Parameters.AddWithValue("@Email", this.Email);
                cmd.Parameters.AddWithValue("@UserID", this.UserID);

                await con.OpenAsync();
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected;
            }
        }
    }
}
