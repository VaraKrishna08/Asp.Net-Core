using Microsoft.AspNetCore.Mvc;
using Demo_Project.Models;
using System.Data.SqlClient;
using System.Data;
using ClosedXML.Excel;
    namespace Demo_Project.Controllers
{
    
    public class UserController : Controller
    {
        private IConfiguration configuration;

        public UserController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region User List
        public IActionResult UserList()
        {
            DataTable table = GetUserData();
            return View(table);
        }
        #endregion

        #region User Delete
        public IActionResult UserDelete(int UserID)
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
                        command.CommandText = "PR_UserDemo_Delete";
                        command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            TempData["SuccessMessage"] = "User deleted successfully.";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "User not found or couldn't be deleted.";
                        }
                    }
                }
            }
            catch (SqlException sqlEx) when (sqlEx.Number == 547)  
            {
                TempData["ErrorMessage"] = "Cannot delete this User because it is referenced by other records (foreign key constraint). Please ensure there are no dependencies before deleting.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
            }

            return RedirectToAction("UserList");
        }
        #endregion

        #region User AddEdit
        public IActionResult UserAddEdit(int UserID)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");

            #region User Drop-Down

            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_UserDemo_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            connection1.Close();

            List<UserDropDownModel> users = new List<UserDropDownModel>();

            foreach (DataRow dataRow in dataTable1.Rows)
            {
                UserDropDownModel userDropDownModel = new UserDropDownModel();
                userDropDownModel.UserID = Convert.ToInt32(dataRow["UserID"]);
                userDropDownModel.UserName = dataRow["UserName"].ToString();
                users.Add(userDropDownModel);
            }

            ViewBag.UserList = users;

            #endregion

            #region User ByID

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_UserDemo_SelectByPK";
            command.Parameters.AddWithValue("@UserID", UserID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            UserModel userModel = new UserModel();

            foreach (DataRow dataRow in table.Rows)
            {
                userModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
                userModel.UserName = @dataRow["UserName"].ToString();
                userModel.Email = @dataRow["Email"].ToString();
                userModel.MobileNo = @dataRow["MobileNo"].ToString();
                userModel.Address = @dataRow["Address"].ToString();
                userModel.IsActive = Convert.ToBoolean(@dataRow["IsActive"]);
                userModel.Password = @dataRow["Password"].ToString(); // Populate the password field
            }

            #endregion

            return View("UserAddEdit", userModel);
        }
        #endregion
        #region User Save
        [HttpPost]
        public IActionResult UserSave(UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = connection.CreateCommand();
                        command.CommandType = CommandType.StoredProcedure;

                        if (userModel.UserID == null)
                        {
                            command.CommandText = "PR_UserDemo_Insert";
                        }
                        else
                        {
                            command.CommandText = "PR_UserDemo_Update";
                            command.Parameters.AddWithValue("@UserID", userModel.UserID);

                            // Only set password if a new one is provided
                            if (!string.IsNullOrWhiteSpace(userModel.Password))
                            {
                                command.Parameters.AddWithValue("@Password", userModel.Password);
                            }
                            else
                            {
                                // Retrieve the existing password
                                var existingPassword = GetUserPassword(userModel.UserID);
                                command.Parameters.AddWithValue("@Password", existingPassword); // Keep existing password
                            }
                        }

                        command.Parameters.AddWithValue("@UserName", userModel.UserName);
                        command.Parameters.AddWithValue("@Email", userModel.Email);
                        command.Parameters.AddWithValue("@MobileNo", userModel.MobileNo);
                        command.Parameters.AddWithValue("@Address", userModel.Address);
                        command.Parameters.AddWithValue("@IsActive", userModel.IsActive);

                        command.ExecuteNonQuery();
                    }

                    TempData["SuccessMessage"] = "User saved successfully!";
                    return RedirectToAction("UserList");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                    return RedirectToAction("UserAddEdit", new { userModel.UserID });
                }
            }

            TempData["ErrorMessage"] = "Please correct the form errors and try again.";
            return View("UserAddEdit", userModel);
        }



        private string GetUserPassword(int? userId)
        {
            string password = string.Empty;
            string connectionString = this.configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("PR_UserDemo_SelectByPK", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", userId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            password = reader["Password"].ToString();
                        }
                    }
                }
            }
            return password;
        }
        #endregion

        #region User Export to Excel
        public async Task<IActionResult> ExportToExcel()
        {
            DataTable data = GetUserData();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Users");
                worksheet.Cell(1, 1).InsertTable(data);
                worksheet.Columns().AdjustToContents();  // Automatically adjust the column widths based on content

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    string fileName = "Users.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
        private DataTable GetUserData()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_UserDemo_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return table;
        }
        #endregion

        [HttpPost]
        public ActionResult Submit(IFormCollection form)
        {
            string UserName = form["User.UserName"];
            string Email = form["User.Email"];
            string Password = form["User.Password"];
            string MobileNo = form["User.MobileNo"];
            string Address = form["User.Address"];
            string IsActive = form["User.IsActive"];

            ViewBag.UserName = UserName;
            ViewBag.Email = Email;
            ViewBag.Password = Password;
            ViewBag.MobileNo = MobileNo;
            ViewBag.Address = Address;
            ViewBag.IsActive = IsActive;

            return View();
        }

        public IActionResult UserLogin(UserLoginModel userLoginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_User_Login";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userLoginModel.UserName;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userLoginModel.Password;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(sqlDataReader);
                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                            HttpContext.Session.SetString("UserName", dr["UserName"].ToString());
                        }

                        return RedirectToAction("index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Login", "User");
                    }

                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return RedirectToAction("Login");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
       
        public IActionResult Login()
        {
            UserLoginModel userLoginModel = new UserLoginModel();
            return View(userLoginModel);
        }
        public IActionResult UserRegister(UserRegisterModel userRegisterModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_User_Register";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userRegisterModel.UserName;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userRegisterModel.Password;
                    sqlCommand.Parameters.Add("@Email", SqlDbType.VarChar).Value = userRegisterModel.Email;
                    sqlCommand.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = userRegisterModel.MobileNo;
                    sqlCommand.Parameters.Add("@Address", SqlDbType.VarChar).Value = userRegisterModel.Address;
                    sqlCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = userRegisterModel.IsActive;

                    sqlCommand.ExecuteNonQuery();
                    return RedirectToAction("Login", "User");
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Register");
            }
            return RedirectToAction("Register");
        }
        public IActionResult Register()
        {
            UserRegisterModel userRegisterModel = new UserRegisterModel();
            return View(userRegisterModel);
        }
    }
}
