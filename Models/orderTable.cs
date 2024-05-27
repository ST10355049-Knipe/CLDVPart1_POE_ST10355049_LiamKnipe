//Liam Knipe St10355049
//REFERENCES: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/ , https://learn.microsoft.com/en-us/aspnet/web-api/overview/advanced/http-cookies
// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-5.0&tabs=visual-studio, https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-5.0
// https://www.codeproject.com/Articles/32545/Exploring-Session-in-ASP-Net , https://www.completecsharptutorial.com/asp-net-mvc5/asp-net-mvc-5-httpget-and-httppost-method-with-example.php#:~:text=HTTPGet%20method%20is%20default%20whereas,body%20of%20the%20HTTP%20request.
// https://dev.to/fabriziobagala/logging-in-aspnet-core-6-3mbh , https://stackoverflow.com/questions/11585/clearing-page-cache-in-asp-net
// https://www.c-sharpcorner.com/article/asp-net-core-3-1-authentication-and-authorization/ , https://www.youtube.com/watch?v=8_eMgS6UszY&list=PLIY8eNdw5tW_ZQawyxK0Dd1cZXwcNFWn8&ab_channel=SimpleSnippets
// https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_bootstrap.htm , https://www.guru99.com/insert-update-delete-asp-net.html#:~:text=The%20'sql'%20statement%20can%20be,are%20specified%20in%20ASP.Net.


using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace ST10355049.Models
{
    public class orderTable
    {
        public static string con_string = "Server=tcp:st10355049.database.windows.net,1433;Initial Catalog=st10355049-Database;Persist Security Info=False;User ID=liamknipe;Password=Lk20040119;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
        public static SqlConnection con = new SqlConnection(con_string);

        public int OrderID { get; set; }
        public int UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        public int InsertOrder(orderTable o)
        {
            try
            {
                string sql = "INSERT INTO orderTable (UserID, OrderDate, TotalAmount, Status) VALUES (@UserID, @OrderDate, @TotalAmount, @Status)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@UserID", o.UserID);
                cmd.Parameters.AddWithValue("@OrderDate", o.OrderDate);
                cmd.Parameters.AddWithValue("@TotalAmount", o.TotalAmount);
                cmd.Parameters.AddWithValue("@Status", o.Status);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();

                return rowsAffected;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For now, rethrow the exception
                throw ex;
            }
        }

        public static List<orderTable> GetOrdersByUserID(int userID)
        {
            List<orderTable> orders = new List<orderTable>();
            using (SqlConnection con = new SqlConnection(con_string))
            {
                string sql = "SELECT * FROM orderTable WHERE UserID = @UserID";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@UserID", userID);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    orderTable order = new orderTable();
                    order.OrderID = Convert.ToInt32(rdr["OrderID"]);
                    order.UserID = Convert.ToInt32(rdr["UserID"]);
                    order.OrderDate = Convert.ToDateTime(rdr["OrderDate"]);
                    order.TotalAmount = Convert.ToDecimal(rdr["TotalAmount"]);
                    order.Status = rdr["Status"].ToString();

                    orders.Add(order);
                }
            }

            return orders;
        }
    }
}