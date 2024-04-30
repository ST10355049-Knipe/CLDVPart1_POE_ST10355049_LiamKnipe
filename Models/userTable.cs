using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ST10355049.Models
{
    public class userTable 
    {
        public static string con_string = "Server=tcp:st10355049.database.windows.net,1433;Initial Catalog=st10355049-Database;Persist Security Info=False;User ID=liamknipe;Password=Lk20040119;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
        public static SqlConnection con = new SqlConnection(con_string);

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

        public string Role { get; set; }

        public async Task<int> InsertUserAsync(userTable user)
        {
            try
            {
                string sql = "INSERT INTO userTable (FirstName, LastName, Email, Password, Role) VALUES (@FirstName, @LastName, @Email, @Password, @Role)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Role", user.Role ?? "Client"); // Default to 'Client' role if not provided

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

        public async static Task<List<userTable>> GetUsersByRoleAsync(string role)
        {
            List<userTable> users = new List<userTable>();
            using (SqlConnection con = new SqlConnection(con_string))
            {
                string sql = "SELECT * FROM userTable WHERE Role = @Role";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Role", role);

                await con.OpenAsync();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (await rdr.ReadAsync())
                {
                    userTable user = new userTable();
                    user.UserID = Convert.ToInt32(rdr["UserID"]);
                    user.FirstName = rdr["FirstName"].ToString();
                    user.LastName = rdr["LastName"].ToString();
                    user.Email = rdr["Email"].ToString();
                    user.Password = rdr["Password"].ToString();
                    user.Role = rdr["Role"].ToString();

                    users.Add(user);
                }
            }

            return users;
        }
    }
}
