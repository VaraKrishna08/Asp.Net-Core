using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using ClosedXML.Excel;
using Demo_Project.Models;
using static Demo_Project.Controllers.UserController;

namespace Demo_Project.Controllers
{
    public class OrderDetailController : Controller
    {
        
        private IConfiguration configuration;

        public OrderDetailController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
       

        #region OrderDetail List
        public IActionResult OrderDetailList()
        {
            DataTable table = GetOrderDetailData();
            return View(table);
        }
        #endregion
        #region OrderDetail Delete

        public IActionResult OrderDetailDelete(int OrderDetailID)
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
                        command.CommandText = "PR_ORDERDETAIL_DELETE";

                        command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = OrderDetailID;

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            TempData["SuccessMessage"] = "OrderDetail deleted successfully!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "No record found with the given ID.";
                        }
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 547) 
            {
                TempData["ErrorMessage"] = "Cannot delete this OrderDetail because it is referenced by other records (foreign key constraint). Please ensure there are no dependencies before deleting.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
            }

            return RedirectToAction("OrderDetailList");
        }
        #endregion
        #region OrderDetail AddEdit

        public IActionResult OrderDetailAddEdit(int OrderDetailID)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection1 = new SqlConnection(connectionString))
            {
                connection1.Open();

                // Fetch Product List
                SqlCommand command1 = connection1.CreateCommand();
                command1.CommandType = CommandType.StoredProcedure;
                command1.CommandText = "PR_Product_DropDown";
                SqlDataReader reader1 = command1.ExecuteReader();
                DataTable dataTable1 = new DataTable();
                dataTable1.Load(reader1);

                if (dataTable1.Rows.Count > 0)
                {
                    List<ProductDropDownModel> productList = new List<ProductDropDownModel>();
                    foreach (DataRow data in dataTable1.Rows)
                    {
                        ProductDropDownModel productDropDownModel = new ProductDropDownModel
                        {
                            ProductID = Convert.ToInt32(data["ProductID"]),
                            ProductName = data["ProductName"].ToString()
                        };
                        productList.Add(productDropDownModel);
                    }
                    ViewBag.ProductList = productList;
                }
                else
                {
                    ViewBag.ProductList = new List<ProductDropDownModel>();
                }

                // Fetch User List
                SqlCommand commandUser = connection1.CreateCommand();
                commandUser.CommandType = CommandType.StoredProcedure;
                commandUser.CommandText = "PR_UserDemo_DropDown";
                SqlDataReader readerUser = commandUser.ExecuteReader();
                DataTable table1 = new DataTable();
                table1.Load(readerUser);
                readerUser.Close();

                if (table1.Rows.Count > 0)
                {
                    List<UserDropDownModel> userList = new List<UserDropDownModel>();
                    foreach (DataRow dr in table1.Rows)
                    {
                        UserDropDownModel model = new UserDropDownModel
                        {
                            UserID = Convert.ToInt32(dr["UserID"]),
                            UserName = dr["UserName"].ToString()
                        };
                        userList.Add(model);
                    }
                    ViewBag.UserList = userList;
                }
                else
                {
                    ViewBag.UserList = new List<UserDropDownModel>();
                }

                // Fetch Order List
                SqlCommand commandOrder = connection1.CreateCommand();
                commandOrder.CommandType = CommandType.StoredProcedure;
                commandOrder.CommandText = "PR_OrderDemo_DropDown";
                SqlDataReader readerOrder = commandOrder.ExecuteReader();
                DataTable tableOrder = new DataTable();
                tableOrder.Load(readerOrder);
                readerOrder.Close();

                if (tableOrder.Rows.Count > 0)
                {
                    List<OrderDemoDropDownModel> orderList = new List<OrderDemoDropDownModel>();
                    foreach (DataRow dr in tableOrder.Rows)
                    {
                        OrderDemoDropDownModel model = new OrderDemoDropDownModel
                        {
                            OrderID = Convert.ToInt32(dr["OrderID"]),
                            OrderNumber = dr["OrderNumber"].ToString()
                        };
                        orderList.Add(model);
                    }
                    ViewBag.OrderList = orderList;
                }
                else
                {
                    ViewBag.OrderList = new List<OrderDemoDropDownModel>();
                }

                // Fetch Order Detail if OrderDetailID is provided
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_OrderDetail_SelectByPK";
                    command.Parameters.AddWithValue("@OrderDetailID", OrderDetailID);
                    SqlDataReader reader = command.ExecuteReader();
                    DataTable table = new DataTable();
                    table.Load(reader);

                    OrderDetailModel orderDetailModel = new OrderDetailModel();
                    foreach (DataRow dataRow in table.Rows)
                    {
                        orderDetailModel.OrderDetailID = Convert.ToInt32(dataRow["OrderDetailID"]);
                        orderDetailModel.OrderID = Convert.ToInt32(dataRow["OrderID"]);
                        orderDetailModel.ProductID = Convert.ToInt32(dataRow["ProductID"]);
                        orderDetailModel.Quantity = Convert.ToInt32(dataRow["Quantity"]);
                        orderDetailModel.Amount = Convert.ToDecimal(dataRow["Amount"]);
                        orderDetailModel.TotalAmount = Convert.ToDecimal(dataRow["TotalAmount"]);
                        orderDetailModel.UserID = Convert.ToInt32(dataRow["UserID"]);
                    }

                    // Null check after populating ViewBag
                    if (ViewBag.ProductList == null || ViewBag.UserList == null)
                    {
                        TempData["ErrorMessage"] = "Unable to load product or user data.";
                        return View("OrderDetailAddEdit", orderDetailModel);
                    }

                    return View("OrderDetailAddEdit", orderDetailModel);
                }
            }
        }
        #endregion
        #region OrderDetail Save

        public IActionResult OrderDetailSave(OrderDetailModel orderDetailModel)
        {
            if (orderDetailModel.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }
            if (orderDetailModel.ProductID <= 0)
            {
                ModelState.AddModelError("ProductID", "A valid Product is required.");
            }
            if (orderDetailModel.OrderID <= 0)
            {
                ModelState.AddModelError("OrderID", "A valid OrderID is required.");
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

                        if (orderDetailModel.OrderDetailID == null)
                        {
                            command.CommandText = "PR_OrderDetail_Insert";
                        }
                        else
                        {
                            command.CommandText = "PR_OrderDetail_Update";
                            command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = orderDetailModel.OrderDetailID;
                        }

                        command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderDetailModel.OrderID;
                        command.Parameters.Add("@ProductID", SqlDbType.Int).Value = orderDetailModel.ProductID;
                        command.Parameters.Add("@Quantity", SqlDbType.Int).Value = orderDetailModel.Quantity;
                        command.Parameters.Add("@Amount", SqlDbType.Decimal).Value = orderDetailModel.Amount;
                        command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = orderDetailModel.TotalAmount;
                        command.Parameters.Add("@UserID", SqlDbType.Int).Value = orderDetailModel.UserID;

                        command.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "OrderDetail saved successfully!!!";

                return RedirectToAction("OrderDetailList");
            }

            return View("OrderDetailAddEdit", orderDetailModel);
        }
        #endregion
        #region OrderDetail Excel

        public async Task<IActionResult> ExportToExcel()
        {
            DataTable data = GetOrderDetailData();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Orders");
                worksheet.Cell(1, 1).InsertTable(data);
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    string fileName = "OrderDetails.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
        #endregion

        #region Private Methods
        private DataTable GetOrderDetailData()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_OrderDetail_SelectAll";
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Load(reader);
                    return table;
                }
            }
        }
        #endregion

        #region Form Handling
        [HttpPost]
        public ActionResult Submit(IFormCollection form)
        {
            // Extract values from the form
            string orderid = form["OrderDetail.OrderID"];
            string productid = form["OrderDetail.ProductID"];
            string quantity = form["OrderDetail.Quantity"];
            string amount = form["OrderDetail.Amount"];
            string totalAmount = form["OrderDetail.TotalAmount"];
            
            // Store values in ViewBag to display them
            ViewBag.OrderDate = orderid;
            ViewBag.ProductID = productid;
            ViewBag.Quantity = quantity;
            ViewBag.Amount = amount;
            ViewBag.TotalAmount = totalAmount;

            return View();
        }
        #endregion
    }
}
