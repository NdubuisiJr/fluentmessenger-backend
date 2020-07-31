using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluentMessenger.API.Entities {
    public class MessageTemplate : EntityBase{
        [Required]
        public string Message { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public int UserId { get; set; }
    }
}
