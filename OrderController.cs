using Demo_Project.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using ClosedXML.Excel;
using static Demo_Project.Controllers.UserController;

namespace Demo_Project.Controllers
{
    public class OrderController : Controller
    {
        private IConfiguration configuration;

        public OrderController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #region Order List
        public IActionResult OrderList()
        {
            DataTable table = GetOrderData();
            return View(table);
        }
        #endregion
       

        #region Delete Order Method

        public IActionResult OrderDelete(int OrderID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_OrderDemo_Delete";
                        command.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            TempData["SuccessMessage"] = "Order deleted successfully.";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Order not found or couldn't be deleted.";
                        }
                    }
                }
            }
            catch (SqlException sqlEx) when (sqlEx.Number == 547)
            {
                TempData["ErrorMessage"] = "Cannot delete this Order because it is referenced by other records (foreign key constraint). Please ensure there are no dependencies before deleting.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("OrderList");
        }

        #endregion

        #region Add/Edit Order Methods

        public IActionResult OrderAddEdit(int OrderID)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            // Load Customer DropDown
            SqlCommand commandCustomer = connection.CreateCommand();
            commandCustomer.CommandType = CommandType.StoredProcedure;
            commandCustomer.CommandText = "PR_Customer_DropDown";
            SqlDataReader readerCustomer = commandCustomer.ExecuteReader();
            DataTable tableCustomer = new DataTable();
            tableCustomer.Load(readerCustomer);
            List<CustomerDropDownModel> customerList = new List<CustomerDropDownModel>();
            foreach (DataRow data in tableCustomer.Rows)
            {
                customerList.Add(new CustomerDropDownModel
                {
                    CustomerID = Convert.ToInt32(data["CustomerID"]),
                    CustomerName = data["CustomerName"].ToString()
                });
            }
            ViewBag.CustomerList = customerList;

            // Load User DropDown
            SqlCommand commandUser = connection.CreateCommand();
            commandUser.CommandType = CommandType.StoredProcedure;
            commandUser.CommandText = "PR_UserDemo_DropDown";
            SqlDataReader readerUser = commandUser.ExecuteReader();
            DataTable tableUser = new DataTable();
            tableUser.Load(readerUser);
            readerUser.Close();

            List<UserDropDownModel> userList = new List<UserDropDownModel>();
            foreach (DataRow dr in tableUser.Rows)
            {
                userList.Add(new UserDropDownModel
                {
                    UserID = Convert.ToInt32(dr["UserID"]),
                    UserName = dr["UserName"].ToString()
                });
            }
            ViewBag.UserList = userList;

            // Load Order data if editing
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_OrderDemo_SelectByPK";
            command.Parameters.AddWithValue("@OrderID", OrderID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            OrderModel orderModel = new OrderModel();

            foreach (DataRow dataRow in table.Rows)
            {
                orderModel.OrderID = Convert.ToInt32(dataRow["OrderID"]);
                orderModel.OrderDate = Convert.ToDateTime(dataRow["OrderDate"]);
                orderModel.OrderNumber = dataRow["OrderNumber"].ToString();
                orderModel.CustomerID = Convert.ToInt32(dataRow["CustomerID"]);
                orderModel.PaymentMode = dataRow["PaymentMode"].ToString();
                orderModel.TotalAmount = Convert.ToDecimal(dataRow["TotalAmount"]);
                orderModel.ShippingAddress = dataRow["ShippingAddress"].ToString();
                orderModel.UserID = Convert.ToInt32(dataRow["UserID"]);
            }

            return View("OrderAddEdit", orderModel);
        }

        public IActionResult OrderSave(OrderModel orderModel)
        {
            if (orderModel.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }

            if (orderModel.CustomerID <= 0)
            {
                ModelState.AddModelError("CustomerID", "A valid Customer is required.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        if (orderModel.OrderID == null)
                        {
                            command.CommandText = "PR_OrderDemo_Insert";
                            command.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value = orderModel.OrderDate;
                            command.Parameters.Add("@OrderNumber", SqlDbType.VarChar).Value = orderModel.OrderNumber;
                            command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = orderModel.CustomerID;
                            command.Parameters.Add("@PaymentMode", SqlDbType.VarChar).Value = orderModel.PaymentMode;
                            command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = orderModel.TotalAmount;
                            command.Parameters.Add("@ShippingAddress", SqlDbType.VarChar).Value = orderModel.ShippingAddress;
                            command.Parameters.Add("@UserID", SqlDbType.Int).Value = orderModel.UserID;
                        }
                        else
                        {
                            command.CommandText = "PR_OrderDemo_Update";
                            command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderModel.OrderID;
                            command.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value = orderModel.OrderDate;
                            command.Parameters.Add("@OrderNumber", SqlDbType.VarChar).Value = orderModel.OrderNumber;
                            command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = orderModel.CustomerID;
                            command.Parameters.Add("@PaymentMode", SqlDbType.VarChar).Value = orderModel.PaymentMode;
                            command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = orderModel.TotalAmount;
                            command.Parameters.Add("@ShippingAddress", SqlDbType.VarChar).Value = orderModel.ShippingAddress;
                            command.Parameters.Add("@UserID", SqlDbType.Int).Value = orderModel.UserID;
                        }

                        command.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Order saved successfully!!!";

                return RedirectToAction("OrderList");
            }
            return View("OrderAddEdit", orderModel);
        }

        #endregion
       
        #region Export Methods


        public IActionResult ExportToExcel()
        {
            DataTable data = GetOrderData();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Order");
                worksheet.Cell(1, 1).InsertTable(data);
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    string fileName = "Order.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        #endregion
        #region Helper Methods

        private DataTable GetOrderData()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_OrderDemo_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return table;
        }

        #endregion

        #region Submit Form Method

        public ActionResult Submit(IFormCollection form)
        {
            // Extract values from the form
            string orderid = form["Order.OrderID"];
            string productid = form["Orded.ProductID"];
            string quantity = form["Orderl.Quantity"];
            string amount = form["Order.Amount"];
            string totalAmount = form["Order.TotalAmount"];

            // Store values in ViewBag to display them
            ViewBag.OrderID = orderid;
            ViewBag.ProductID = productid;
            ViewBag.Quantity = quantity;
            ViewBag.Amount = amount;
            ViewBag.TotalAmount = totalAmount;

            return View();
        }

        #endregion

    }
}
