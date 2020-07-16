using System;

namespace FluentMessenger.API.Dtos {
    /// <summary>
    /// The object that represents a message and a draft
    /// </summary>
    public class MessageDto {  
        /// <summary>
        /// The mesage Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The message body
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The sent time
        /// </summary>
        public DateTime SentTime { get; set; }

        /// <summary>
        /// A flag that checks if it is draft or a message
        /// </summary>
        public bool IsDraft { get; set; }
    }
}
