using System.ComponentModel.DataAnnotations;

namespace FluentMessenger.API.Dtos {
    /// <summary>
    /// The input format for creating a user
    /// </summary>
    public class UserForCreationDto {
        /// <summary>
        /// User's surname
        /// </summary>
        [Required(ErrorMessage ="Please enter your surname")]
        [StringLength(100)]
        public string Surname { get; set; }

        /// <summary>
        /// User's other names
        /// </summary>
        [StringLength(100)]
        [Required]
        public string OtherNames { get; set; }

        /// <summary>
        /// User's email address
        /// </summary>
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        /// <summary>
        /// User's phone number. In Nigerian format (without country code)
        /// </summary>
        [Required(ErrorMessage ="Enter your phone number")]
        [Phone(ErrorMessage ="Enter a valid phone number")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        /// <summary>
        /// User's password with minimum length of 6 and max of 50
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters. And can't be greater than 50 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Same value provided in the password field
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters. And can't be greater than 50 characters.")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ComfirmPassword { get; set; }
    }
}
