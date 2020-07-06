using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FluentMessenger.API.Entities {
    public class User : EntityBase {
        [Required]
        public string Surname { get; set; }
        
        [Required]
        public string OtherNames { get; set; }
        
        [Required]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Password { get; set; }

        public decimal SMSCredit { get; set; }

        public bool IsVerified { get; set; }

        public int VerificationCode { get; set; }

        public virtual IEnumerable<Group> Groups { get; set; }
    }
}
