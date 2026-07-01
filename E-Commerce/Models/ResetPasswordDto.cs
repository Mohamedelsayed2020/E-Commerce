using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email is required"), MaxLength(100), EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = "";
        
        [Required(ErrorMessage = "Password is required"), MaxLength(100)]
        public string Password { get; set; } = "";
        [Required(ErrorMessage = "confirme password is required"), MaxLength(100), Compare("Password", ErrorMessage = "Password and Confirm Password must be same")]
        public string ConfirmPassword { get; set; } = "";
    }
}
