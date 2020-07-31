using System.Collections.Generic;

namespace FluentMessenger.API.Dtos {
    /// <summary>
    /// The contact message object that is used to form the joining table
    /// </summary>
    public class ContactMessageDto {
        /// <summary>
        /// A collection of contacts
        /// </summary>
        public IEnumerable<ContactDto> ContactsDtos { get; set; }

        /// <summary>
        /// A collection of messages
        /// </summary>
        public IEnumerable<MessageDto> MessageDtos { get; set; }

        /// <summary>
        /// Checks to ensure that one of the collection is null
        /// </summary>
        /// <returns></returns>
        public bool IsConsistent() {
            if (ContactsDtos is { } && MessageDtos is { }) {
                return false;
            }
            return true;
        }
    }
}
