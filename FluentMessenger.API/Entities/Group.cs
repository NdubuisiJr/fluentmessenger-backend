using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluentMessenger.API.Entities {
    public class Group : EntityBase {
        public string Title { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public int UserId { get; set; }

        public virtual List<Contact> Contacts { get; set; }

        public virtual List<Message> Messages { get; set; }
    }
}
