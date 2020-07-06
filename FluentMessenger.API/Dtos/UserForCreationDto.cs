using System.ComponentModel.DataAnnotations;

namespace FluentMessenger.API.Dtos {
    public class UserForCreationDto {
        [Required(ErrorMessage ="Please enter your surname")]
        [StringLength(100)]
        public string Surname { get; set; }

        [StringLength(100)]
        public string OtherNames { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Enter your phone number")]
        [Phone(ErrorMessage ="Enter a valid phone number")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters. And can't be greater than 50 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters. And can't be greater than 50 characters.")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ComfirmPassword { get; set; }
    }
}
