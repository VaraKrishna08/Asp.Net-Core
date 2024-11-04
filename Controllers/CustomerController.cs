using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using ClosedXML.Excel;
using Demo_Project.Models;
using static Demo_Project.Controllers.UserController;

namespace Demo_Project.Controllers
{
    public class CustomerController : Controller
    {
       
        private IConfiguration configuration;
       
        public CustomerController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
     

        #region Customer List
        public IActionResult CustomerList()
        {
            DataTable table = GetCustomerData();
            return View(table);
        }
        #endregion
        #region Customer delete

        public IActionResult CustomerDelete(int CustomerID)
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
                        command.CommandText = "PR_Customer_Delete";
                        command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = CustomerID;
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            TempData["SuccessMessage"] = "Customer deleted successfully.";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Customer not found or couldn't be deleted.";
                        }
                    }
                }
            }
            catch (SqlException sqlEx) when (sqlEx.Number == 547)  
            {
                TempData["ErrorMessage"] = "Cannot delete this Customer because it is referenced by other records (foreign key constraint). Please ensure there are no dependencies before deleting.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
            }

            return RedirectToAction("CustomerList");
        }
        #endregion
        #region Customer Add Edit


        public IActionResult CustomerAddEdit(int CustomerID)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");

            // Fetch User List
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
            }

            // Fetch Customer Data if CustomerID is provided
            CustomerModel customerModel = new CustomerModel();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Customer_SelectByPK";
                command.Parameters.AddWithValue("@CustomerID", CustomerID);
                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);

                foreach (DataRow dataRow in table.Rows)
                {
                    customerModel.CustomerID = Convert.ToInt32(dataRow["CustomerID"]);
                    customerModel.CustomerName = dataRow["CustomerName"].ToString();
                    customerModel.HomeAddress = dataRow["HomeAddress"].ToString();
                    customerModel.Email = dataRow["Email"].ToString();
                    customerModel.MobileNo = dataRow["MobileNo"].ToString();
                    customerModel.GST_NO = dataRow["GST_NO"].ToString();
                    customerModel.CityName = dataRow["CityName"].ToString();
                    customerModel.Pincode = dataRow["PinCode"].ToString();
                    customerModel.NetAmount = Convert.ToDecimal(dataRow["NetAmount"]);
                    customerModel.UserID = Convert.ToInt32(dataRow["UserID"]);
                }
            }

            return View("CustomerAddEdit", customerModel);
        }
        #endregion
        #region Customer Save

        public IActionResult CustomerSave(CustomerModel customerModel)
        {
            if (customerModel.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;

                    if (customerModel.CustomerID == null)
                    {
                        command.CommandText = "PR_Customer_Insert";
                    }
                    else
                    {
                        command.CommandText = "PR_Customer_Update";
                        command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerModel.CustomerID;
                    }

                    command.Parameters.Add("@CustomerName", SqlDbType.VarChar).Value = customerModel.CustomerName;
                    command.Parameters.Add("@HomeAddress", SqlDbType.VarChar).Value = customerModel.HomeAddress;
                    command.Parameters.Add("@Email", SqlDbType.VarChar).Value = customerModel.Email;
                    command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = customerModel.MobileNo;
                    command.Parameters.Add("@GST_NO", SqlDbType.VarChar).Value = customerModel.GST_NO;
                    command.Parameters.Add("@CityName", SqlDbType.VarChar).Value = customerModel.CityName;
                    command.Parameters.Add("@PinCode", SqlDbType.VarChar).Value = customerModel.Pincode;
                    command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = customerModel.NetAmount;
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = customerModel.UserID;

                    command.ExecuteNonQuery();
                    TempData["SuccessMessage"] = "Customer saved successfully!!!";
                }

                return RedirectToAction("CustomerList");
            }

            return View("CustomerAddEdit", customerModel);
        }
        #endregion
        #region Customer excel

        public async Task<IActionResult> ExportToExcel()
        {
            DataTable data = GetCustomerData();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Customers");
                worksheet.Cell(1, 1).InsertTable(data);
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    string fileName = "Customers.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
        #endregion

        #region Private Methods
        private DataTable GetCustomerData()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Customer_SelectAll";
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
            string customerName = form["Customer.CustomerName"];
            string homeAddress = form["Customer.HomeAddress"];
            string email = form["Customer.Email"];
            string mobileNo = form["Customer.MobileNo"];
            string GST_NO = form["Customer.GST_NO"];
            string cityName = form["Customer.CityName"];
            string pinCode = form["Customer.PinCode"];
            string netAmount = form["Customer.NetAmount"];

            // Store values in ViewBag to display them
            ViewBag.CustomerName = customerName;
            ViewBag.HomeAddress = homeAddress;
            ViewBag.Email = email;
            ViewBag.MobileNo = mobileNo;
            ViewBag.GST_NO = GST_NO;
            ViewBag.CityName = cityName;
            ViewBag.PinCode = pinCode;
            ViewBag.NetAmount = netAmount;

            return View();
        }
        #endregion
    }
}
