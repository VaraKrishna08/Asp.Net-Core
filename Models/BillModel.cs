using System.ComponentModel.DataAnnotations;

namespace Demo_Project.Models
{
    public class BillModel
    {
        public int? BillID { get; set; }

        [Required(ErrorMessage = "Please Enter Bill No ")]
       
        public string BillNumber { get; set; }
        // Remove the [Required] attribute here
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? BillDate { get; set; }



        [Required]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Please Enter Total Amount")]
        
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Please Enter Discount ")]
        
        public decimal Discount { get; set; }

        [Required(ErrorMessage = "Please Enter Net Amount")]
     
        public decimal NetAmount { get; set; }

        [Required]
        public int UserID { get; set; }
    }
}
