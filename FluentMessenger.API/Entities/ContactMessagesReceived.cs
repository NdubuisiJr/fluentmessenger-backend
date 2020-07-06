using FluentMessenger.API.Interfaces;

namespace FluentMessenger.API.Entities {
    public class ContactMessagesReceived  {
        public int ContactId { get; set; }
        public Contact Contact { get; set; }
        public int MessageId { get; set; }
        public Message Message { get; set; }
    }
}
