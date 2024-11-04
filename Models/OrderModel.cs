using System.ComponentModel.DataAnnotations;

namespace Demo_Project.Models
{
    public class OrderModel
    {
        public int? OrderID { get; set; }
        public string OrderNumber { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM.DD.YYYY}")]
        public DateTime OrderDate { get; set; }
        [Required(ErrorMessage = "Please select a Customer")]
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Please Enter Payment Mode")]
        
        public string PaymentMode { get; set; }

        [Required(ErrorMessage = "Please Enter Total Amount")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Please Enter Shipping Address")]
        [MaxLength(50)]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "Please select a User")]
        public int UserID { get; set; }
    }

    public class CustomerDropDownModel
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
    }
}

//using System.ComponentModel.DataAnnotations;

//namespace Demo_Project.Models
//{
//    public class OrderModel
//    {
//        public int OrderID { get; set; }


//        [DisplayFormat(DataFormatString = "{0:MM.DD.YYYY}")]
//        public DateTime OrderDate { get; set; }

//        //[Required]
//        public int CustomerID { get; set; }

//        [Required(ErrorMessage = "Please Enter PaymentMode ")]
//        
//        public string PaymentMode { get; set; }

//        [Required(ErrorMessage = "Please Enter Total Amount ")]
//        
//        public decimal TotalAmount { get; set; }

//        [Required(ErrorMessage = "Please Enter Shipping Address ")]
//        [MaxLength(50)]
//        public string ShippingAddress { get; set; }

//        [Required]
//        public int UserID { get; set; }
//    }

//    public class CustomerDropDownModel
//    {
//        public int CustomerID { get; set; }
//        public string CustomerName { get; set; }
//    }
//}
