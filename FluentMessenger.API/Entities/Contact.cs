using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluentMessenger.API.Entities {
    public class Contact : EntityBase {
        public string FullName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [ForeignKey("GroupId")]
        public Group Group { get; set; }
        public int GroupId { get; set; }
    }
}
