using System;

namespace FluentMessenger.API.Dtos {
    public class MessageDto {  
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime SentTime { get; set; }
        public bool IsDraft { get; set; }
    }
}
