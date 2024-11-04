using System.ComponentModel.DataAnnotations;

namespace Demo_Project.Models
{
    public class CustomerModel
    {
        public int? CustomerID { get; set; }
        [Required(ErrorMessage = "Please Enter Customer Name")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Please Enter Home Address")]
        public string HomeAddress { get; set; }
        [Required(ErrorMessage = "Please Enter Email")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Your Email is not valid.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please Enter Mobile Number")]
        
        public string MobileNo { get; set; }
        [Required(ErrorMessage = "Please Enter GST Number")]
        [MaxLength(14)]
        public string GST_NO { get; set; }
        [Required(ErrorMessage = "Please Enter City Name")]
        public string CityName { get; set; }
        [Required(ErrorMessage = "Please Enter Pin Code")]
        [MaxLength(6)]
        public string Pincode { get; set; }
        [Required(ErrorMessage = "Please Enter Net Amount")]
        public decimal NetAmount { get; set; }
        public int UserID { get; set; }

    }
}
