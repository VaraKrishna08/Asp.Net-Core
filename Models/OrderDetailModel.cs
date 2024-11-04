using System.ComponentModel.DataAnnotations;

namespace Demo_Project.Models
{
    public class OrderDetailModel
    {
        public int? OrderDetailID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Please Enter Quality")]
       
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Please Enter  Amount")]
      
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please Enter Total Amount")]
       
        public decimal TotalAmount { get; set; }

        [Required]
        public int UserID { get; set; }

    }
    public class ProductDropDownModel()
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
    }
    public class OrderDemoDropDownModel()
    {
        public int OrderID { get; set; }
        public string OrderNumber{get;set;}
    }

}
