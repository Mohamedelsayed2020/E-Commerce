using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models
{
    public class ProfileDto
    {
        [Required(ErrorMessage = "First Name is Requried"), MaxLength(100)]
        public string FirstName { get; set; } = "";
        [Required(ErrorMessage = "Last Name is Requried"), MaxLength(100)]
        public string LastName { get; set; } = "";
        [Required, MaxLength(100), EmailAddress]
        public string Email { get; set; } = "";
        [Phone(ErrorMessage = "format is not valid"), MaxLength(20)]
        public string? PhoneNumber { get; set; }
        [Required, MaxLength(200)]
        public string Address { get; set; } = "";
    }
}
