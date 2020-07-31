using System.ComponentModel.DataAnnotations;

namespace FluentMessenger.API.Dtos {
    /// <summary>
    /// The required object used for verifying a user's account
    /// </summary>
    public class UserForVerificationDto {

        /// <summary>
        /// The verification code sent to the user's contact details
        /// </summary>
        [Required]
        public int VerificationCode { get; set; }

        /// <summary>
        /// The User's email address
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// The user's phone number
        /// </summary>
        [Required]
        public string Phone { get; set; }

        /// <summary>
        /// A flag to checck whether is a first time verification or it's for a password reset verification
        /// </summary>
        [Required]
        public bool IsPasswordReset { get; set; }
    }
}
