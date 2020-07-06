using System.Collections.Generic;

namespace FluentMessenger.API.Dtos {
    public class ContactMessageDto {
        public IEnumerable<ContactDto> ContactsDtos { get; set; }
        public IEnumerable<MessageDto> MessageDtos { get; set; }

        public bool IsConsistent() {
            if (ContactsDtos is { } && MessageDtos is { }) {
                return false;
            }
            return true;
        }
    }
}
