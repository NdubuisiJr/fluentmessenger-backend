
using System.Collections.Generic;

namespace FluentMessenger.API.Dtos {
    public class GroupDto {
        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<MessageDto> Messages { get; set; }
        public IEnumerable<ContactDto> Contacts { get; set; }
    }
}
