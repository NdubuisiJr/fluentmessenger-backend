using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluentMessenger.API.Entities {
    public class Message : EntityBase {
        
        [Required]
        public string Value { get; set; }

        [Required]
        public DateTime SentTime { get; set; }

        [ForeignKey("GroupId")]
        public Group Group { get; set; }
        public int GroupId { get; set; }
    }
}
