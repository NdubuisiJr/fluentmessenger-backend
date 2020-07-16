using System.ComponentModel.DataAnnotations;

namespace FluentMessenger.API.Dtos {
    /// <summary>
    /// This is the object required for changing a user's password
    /// </summary>
    public class PasswordResetDto {

        /// <summary>
        /// The user's email
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// The user's phone number
        /// </summary>
        [Required]
        public string Phone { get; set; }
        
    }
}
