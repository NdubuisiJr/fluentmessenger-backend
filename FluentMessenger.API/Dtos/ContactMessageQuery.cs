using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluentMessenger.API.Dtos {
    public class ContactMessageQuery {
        public int MessageId { get; set; }
        public int ContactId { get; set; }

        public bool IsConsistent() {
            if (MessageId == ContactId) {
                return false;
            }
            if (MessageId > 0 && ContactId>0) {
                return false;
            }
            return true;
        }
    }
}
