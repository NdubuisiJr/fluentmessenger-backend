using System.ComponentModel.DataAnnotations;

namespace FluentMessenger.API.Dtos {
    public class RetrieveUserDto {
        [Required(ErrorMessage ="Email is Required")]
        [EmailAddress(ErrorMessage ="Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Password is required")]
        [StringLength(50, MinimumLength =6,ErrorMessage="Password must be at least 6 characters. And can't be greater than 50 characters.")]
        [DataType(DataType.Password)]
        public string PassWord { get; set; }
    }
}
