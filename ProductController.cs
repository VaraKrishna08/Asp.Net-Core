using Microsoft.AspNetCore.Mvc;
using Demo_Project.Models;
using System.Data.SqlClient;
using System.Data;
using ClosedXML.Excel;
using static Demo_Project.Controllers.UserController;
namespace Demo_Project.Controllers
{
    public class ProductController : Controller
    {
        private IConfiguration configuration;

        public ProductController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        //public static List<ProductModel> products = new List<ProductModel>
        //{
        //    new ProductModel{ProductID = 1,ProductName="Laptop", ProductPrice=75000,ProductCode="101",Description="Electrnoic device",UserID=1},
        //    new ProductModel{ProductID = 2,ProductName="Mobile", ProductPrice=40000,ProductCode="102",Description="Smart Phone",UserID=1},
        //    new ProductModel{ProductID = 3,ProductName="Airbuds", ProductPrice=2000,ProductCode="103",Description="Hearing device",UserID=1}
        //};
        #region Product list 
        public IActionResult ProductList()
        {
            DataTable table = GetProductData();
            return View(table);
        }
        #endregion
        #region ProductDelete
        public IActionResult ProductDelete(int ProductID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Product_Delete";
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = ProductID;
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    TempData["SuccessMessage"] = "Product deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Product not found or couldn't be deleted.";
                }

            }
            catch (SqlException sqlEx) when (sqlEx.Number == 547)  
            {
                TempData["ErrorMessage"] = "Cannot delete this Product because it is referenced by other records (foreign key constraint). Please ensure there are no dependencies before deleting.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
            }

            return RedirectToAction("ProductList");
        }

        #endregion

        #region Product AddEdit
        public IActionResult ProductAddEdit(int ProductID)
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

            #region ProductByID

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Product_SelectByPK";
            command.Parameters.AddWithValue("@ProductID", ProductID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            ProductModel productModel = new ProductModel();

            foreach (DataRow dr in table.Rows)
            {
                productModel.ProductID = Convert.ToInt32(@dr["ProductID"]);
                productModel.ProductName = @dr["ProductName"].ToString();
                productModel.ProductCode = @dr["ProductCode"].ToString();
                productModel.ProductPrice = Convert.ToDecimal(@dr["ProductPrice"]);
                productModel.Description = @dr["Description"].ToString();
                productModel.UserID = Convert.ToInt32(@dr["UserID"]);
            }

            #endregion

            return View("ProductAddEdit", productModel);
        }
        #endregion
        #region Product Save
        public IActionResult ProductSave(ProductModel productModel)
        {
            if (productModel.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (productModel.ProductID == null)
                {
                    command.CommandText = "PR_Product_Insert";
                }
                else
                {
                    command.CommandText = "PR_Product_Update";
                    command.Parameters.Add("@ProductID", SqlDbType.Int).Value = productModel.ProductID;
                }
                command.Parameters.Add("@ProductName", SqlDbType.VarChar).Value = productModel.ProductName;
                command.Parameters.Add("@ProductCode", SqlDbType.VarChar).Value = productModel.ProductCode;
                command.Parameters.Add("@ProductPrice", SqlDbType.Decimal).Value = productModel.ProductPrice;
                command.Parameters.Add("@Description", SqlDbType.VarChar).Value = productModel.Description;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = productModel.UserID;
                command.ExecuteNonQuery();

                TempData["SuccessMessage"] = "Product saved successfully!!!";


                return RedirectToAction("ProductList");
            }

            return View("ProductAddEdit", productModel);
        }
        #endregion
        #region Product Excel
        public async Task<IActionResult> ExportToExcel()
        {
            DataTable data = GetProductData();

            using (var workbook = new XLWorkbook())
            {
                //Console.WriteLine("workbook:- " + workbook);
                var worksheet = workbook.Worksheets.Add("Products");
                //Console.WriteLine("worksheet:- " + worksheet);
                worksheet.Cell(1, 1).InsertTable(data);
                worksheet.Columns().AdjustToContents();  // Automatically adjust the column widths based on content

                using (var stream = new MemoryStream())
                {
                    //Console.WriteLine("stream:- " + stream);
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    string fileName = "Products.xlsx";
                    Console.WriteLine(stream.ToArray());
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
        private DataTable GetProductData()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Product_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return table;
        }
        #endregion
        [HttpPost]
        public ActionResult Submit(IFormCollection form)
        {
            // Extract values from the form
            string productName = form["Product.ProductName"];
            string productPrice = form["Product.ProductPrice"];
            string productCode = form["Product.ProductCode"];
            string description = form["Product.Description"];

            // Store values in ViewBag to display them
            ViewBag.ProductName = productName;
            ViewBag.ProductPrice = productPrice;
            ViewBag.ProductCode = productCode;
            ViewBag.Description = description;

            return View();
        }

    }
}
