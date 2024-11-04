using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Demo_Project.Models;
using ClosedXML.Excel;
using static Demo_Project.Controllers.UserController;

namespace Demo_Project.Controllers
{
    
    public class BillController : Controller
    {
        private readonly IConfiguration configuration;

        public BillController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region BillList

        public IActionResult BillList()
        {
            DataTable table = GetBillData();
            return View(table);
        }

        #endregion
        #region BillDelete

        public IActionResult BillDelete(int BillID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Bills_Delete";
                    command.Parameters.Add("@BillID", SqlDbType.Int).Value = BillID;
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        TempData["SuccessMessage"] = "Bills deleted successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Bills not found or couldnt be deleted.";
                    }
                }
            }
            catch (SqlException sqlEx) when (sqlEx.Number == 547)  
            {
                TempData["ErrorMessage"] = "Cannot delete this Bill because it is referenced by other records (foreign key constraint). Please ensure there are no dependencies before deleting.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
            }
            return RedirectToAction("BillList");
        }

        #endregion

        #region BillAddEdit

        public IActionResult BillAddEdit(int BillID)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");

            // Load User dropdown
            using (SqlConnection connection1 = new SqlConnection(connectionString))
            {
                connection1.Open();
                SqlCommand command1 = connection1.CreateCommand();
                command1.CommandType = CommandType.StoredProcedure;
                command1.CommandText = "PR_UserDemo_DropDown";
                SqlDataReader reader1 = command1.ExecuteReader();
                DataTable dataTable1 = new DataTable();
                dataTable1.Load(reader1);
                List<UserDropDownModel> userList = new List<UserDropDownModel>();
                foreach (DataRow data in dataTable1.Rows)
                {
                    UserDropDownModel userDropDownModel = new UserDropDownModel
                    {
                        UserID = Convert.ToInt32(data["UserID"]),
                        UserName = data["UserName"].ToString()
                    };
                    userList.Add(userDropDownModel);
                }
                ViewBag.UserList = userList;

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
            }

            BillModel billModel = new BillModel();

            // If BillID is provided, load bill details (edit mode)
            if (BillID>0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Bills_SelectByPK";
                    command.Parameters.AddWithValue("@BillID", BillID);
                    SqlDataReader reader = command.ExecuteReader();
                    DataTable table = new DataTable();
                    table.Load(reader);

                    if (table.Rows.Count > 0)
                    {
                        DataRow dataRow = table.Rows[0];
                        billModel.BillID = Convert.ToInt32(dataRow["BillID"]);
                        billModel.BillNumber = dataRow["BillNumber"].ToString();
                        billModel.BillDate = Convert.ToDateTime(dataRow["BillDate"]);
                        billModel.OrderID = Convert.ToInt32(dataRow["OrderID"]);
                        billModel.TotalAmount = Convert.ToDecimal(dataRow["TotalAmount"]);
                        billModel.Discount = Convert.ToDecimal(dataRow["Discount"]);
                        billModel.NetAmount = Convert.ToDecimal(dataRow["NetAmount"]);
                        billModel.UserID = Convert.ToInt32(dataRow["UserID"]);
                    }
                }
            }

            return View("BillAddEdit", billModel);
        }

        #endregion

       

        #region BillSave

        public IActionResult BillSave(BillModel billModel)
        {
            if (billModel.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }

            if (ModelState.IsValid)
            {
                if (!billModel.BillDate.HasValue)
                {
                    billModel.BillDate = DateTime.Now;
                }

                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;

                    if (billModel.BillID == null)
                    {
                        command.CommandText = "PR_Bills_Insert";
                        command.Parameters.Add("@BillNumber", SqlDbType.VarChar).Value = billModel.BillNumber;
                        command.Parameters.Add("@OrderID", SqlDbType.Int).Value = billModel.OrderID;
                        command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = billModel.TotalAmount;
                        command.Parameters.Add("@Discount", SqlDbType.Decimal).Value = billModel.Discount;
                        command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = billModel.NetAmount;
                        command.Parameters.Add("@UserID", SqlDbType.Int).Value = billModel.UserID;
                    }
                    else
                    {
                        command.CommandText = "PR_Bills_Update";
                        command.Parameters.Add("@BillID", SqlDbType.Int).Value = billModel.BillID;
                        command.Parameters.Add("@BillNumber", SqlDbType.VarChar).Value = billModel.BillNumber;
                        command.Parameters.Add("@BillDate", SqlDbType.DateTime).Value = billModel.BillDate;
                        command.Parameters.Add("@OrderID", SqlDbType.Int).Value = billModel.OrderID;
                        command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = billModel.TotalAmount;
                        command.Parameters.Add("@Discount", SqlDbType.Decimal).Value = billModel.Discount;
                        command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = billModel.NetAmount;
                        command.Parameters.Add("@UserID", SqlDbType.Int).Value = billModel.UserID;
                    }

                    command.ExecuteNonQuery();
                    TempData["SuccessMessage"] = "Bill saved successfully!";
                    return RedirectToAction("BillList");
                }
            }

            return View("BillAddEdit", billModel);
        }

        #endregion

        #region ExportToExcel

        public async Task<IActionResult> ExportToExcel()
        {
            DataTable data = GetBillData();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Bills");
                worksheet.Cell(1, 1).InsertTable(data);
                worksheet.Columns().AdjustToContents();  // Automatically adjust the column widths based on content

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    string fileName = "Bills.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        private DataTable GetBillData()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Bills_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return table;
        }

        #endregion
        public ActionResult Submit(IFormCollection form)
        {
            // Extract values from the form for Bill
            string billNumber = form["Bill.BillNumber"];
            string billDate = form["Bill.BillDate"];
            string orderID = form["Bill.OrderID"];
            string totalAmount = form["Bill.TotalAmount"];
            string discount = form["Bill.Discount"];
            string netAmount = form["Bill.NetAmount"];
            string userID = form["Bill.UserID"];

            // Store values in ViewBag to display them on the view
            ViewBag.BillNumber = billNumber;
            ViewBag.BillDate = billDate;
            ViewBag.OrderID = orderID;
            ViewBag.TotalAmount = totalAmount;
            ViewBag.Discount = discount;
            ViewBag.NetAmount = netAmount;
            ViewBag.UserID = userID;

            // Return the view with the values stored in ViewBag
            return View();
        }

    }
}
