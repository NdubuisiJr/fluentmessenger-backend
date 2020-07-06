using System.ComponentModel.DataAnnotations;

namespace FluentMessenger.API.Dtos {
    public class PasswordResetDto {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        
    }
}
