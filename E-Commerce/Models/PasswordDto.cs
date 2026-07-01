using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models
{
    public class PasswordDto
    {
        [Required(ErrorMessage = "The Current Password is Required"), MaxLength(100)]
        public string CurrentPassword { get; set; } = "";

        [Required(ErrorMessage ="The New Password is Required"), MaxLength(100)]
        public string NewPassword { get; set; } = "";
        [Required(ErrorMessage = "confirme Password is required"), MaxLength(100), Compare("NewPassword", ErrorMessage = "Password and Confirm Password must be same")]
        public string ConfirmPassword { get; set; } = "";
    }
}
