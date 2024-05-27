//Liam Knipe ST10355049
//References: https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-8.0

using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace CloudDevelopment.Models
{
    public class LoginModel
    {
        public static string ConnectionString = "Server=tcp:st10355049.database.windows.net,1433;Initial Catalog=st10355049-Database;Persist Security Info=False;User ID=liamknipe;Password=Lk20040119;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
        public static SqlConnection con = new SqlConnection(ConnectionString);
        public int? ValidateUser(string email, string password)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                string sql = "SELECT UserID, Password FROM userTable WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Email", email);

                try
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string hashedPassword = reader["Password"].ToString();
                            if (VerifyHashedPassword(hashedPassword, password))
                            {
                                // User authenticated successfully, return their ID
                                return int.Parse(reader["UserID"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it appropriately
                    // For now, rethrow the exception
                    throw ex;
                }
            }

            // User authentication failed, return null
            return null;
        }

        private bool VerifyHashedPassword(string hashedPassword, string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return hashedPassword.Equals(builder.ToString());
            }
        }
    }
}
