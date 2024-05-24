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