using System.ComponentModel.DataAnnotations;

namespace FluentMessenger.API.Dtos {
    /// <summary>
    /// The data object used for update a user
    /// </summary>
    public class UserForUpdateDto {

        /// <summary>
        /// The user email's
        /// </summary>
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        /// <summary>
        /// The user's phone number
        /// </summary>
        [Required(ErrorMessage = "Enter your phone number")]
        [Phone(ErrorMessage = "Enter a valid phone number")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        
        /// <summary>
        /// The user's password
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(int.MaxValue,MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters. And can't be greater than 50 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// The same value passed in the password field
        /// </summary>
        public string ComfirmPassword { get; set; }

        /// <summary>
        /// The user's smscredit
        /// </summary>
        public decimal SMSCredit { get; set; }

        /// <summary>
        /// The new notification object
        /// </summary>
        public NotificationDto NotificationDto { get; set; }
    }
}
