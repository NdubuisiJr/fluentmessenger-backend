using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FluentMessenger.API.Dtos {
    public class GroupForCreationDto {

        [Required(ErrorMessage = "Please enter a title")]
        [StringLength(100)]
        public string Title { get; set; }

        public List<ContactDto> Contacts { get; set; }

        public List<MessageDto> Messages { get; set; }
    }
}
