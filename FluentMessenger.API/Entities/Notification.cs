
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluentMessenger.API.Entities {
    public class Notification : EntityBase{
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int UserId { get; set; }

        public DateTime Time { get; set; }
        public string Text { get; set; }
        public bool IsViewed { get; set; }
    }
}
