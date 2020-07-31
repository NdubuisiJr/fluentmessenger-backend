using System.ComponentModel.DataAnnotations.Schema;

namespace FluentMessenger.API.Entities {
    public class Sender : EntityBase {
        
        public string SenderId { get; set; }
        public bool IsApproved { get; set; }
        /// <summary>
        /// The mapping to the sms key that was used to request the sender id.
        /// This value is passed by the client by checking the last 4 characters of the
        /// sms key that was used to send the request.
        /// if it ends with (Jx9c) pass 1. if it is (glIb) pass 2
        /// </summary>
        public int KeyId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
