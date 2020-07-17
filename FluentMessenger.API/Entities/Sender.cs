using System.ComponentModel.DataAnnotations.Schema;

namespace FluentMessenger.API.Entities {
    public class Sender : EntityBase {
        
        public string SenderId { get; set; }
        public bool IsApproved { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
