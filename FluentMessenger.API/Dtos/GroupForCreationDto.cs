using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FluentMessenger.API.Dtos {
    /// <summary>
    /// The object required for creating a group
    /// </summary>
    public class GroupForCreationDto {

        /// <summary>
        /// The title for the group to be created. It is not unique
        /// </summary>
        [Required(ErrorMessage = "Please enter a title")]
        [StringLength(100)]
        public string Title { get; set; }

        /// <summary>
        /// An optional list of contacts
        /// </summary>
        public List<ContactDto> Contacts { get; set; }

        /// <summary>
        /// An optional list of messages
        /// </summary>
        public List<MessageDto> Messages { get; set; }
    }
}
