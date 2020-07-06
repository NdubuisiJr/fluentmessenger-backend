using FluentMessenger.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluentMessenger.API.Entities {
    public class ContactMessagesNotReceived {
        public int ContactId { get; set; }
        public Contact Contact { get; set; }
        public int MessageId { get; set; }
        public Message Message { get; set; }
    }
}
