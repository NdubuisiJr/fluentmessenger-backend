using System.ComponentModel.DataAnnotations;

namespace FluentMessenger.API.Dtos {
    public class UserForVerificationDto {
        [Required]
        public int VerificationCode { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public bool IsPasswordReset { get; set; }
    }
}
