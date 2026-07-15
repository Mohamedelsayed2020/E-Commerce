using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models
{
    public class CheckOutDto
    {
        [Required(ErrorMessage ="Devlivery Address Requried")]
        [MaxLength(200)]
        public string DeliveryAddree { get; set; } = "";
        public string PaymentMethod { get; set; } = "";


    }
}
