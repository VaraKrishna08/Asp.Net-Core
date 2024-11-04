using System.ComponentModel.DataAnnotations;

namespace Demo_Project.Models
{
    public class ProductModel
    {
        public int? ProductID { get; set; }

        [Required(ErrorMessage = "Please Enter Product Name")]
        
        public string ProductName { get; set; }


        public decimal ProductPrice { get; set; }

        [Required(ErrorMessage = "Please Enter Product Code")]
        public string ProductCode { get; set; }

        [Required(ErrorMessage = "Please Enter Product Description")]
        public string Description { get; set; }

        [Required]
        public int UserID { get; set; }
    }
    public class UserDropDownModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
}
