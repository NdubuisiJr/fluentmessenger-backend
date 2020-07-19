using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FluentMessenger.API.Dtos {
    /// <summary>
    /// The object used for requesting a sender Id
    /// </summary>
    public class SenderIdDto {
        /// <summary>
        /// The senderId being requested for. Must be less that or equal to 9 char
        /// </summary>
        [StringLength(9)]
        public string SenderId { get; set; }

        /// <summary>
        /// The corresponding key
        /// </summary>
        public int KeyId { get; set; }

        /// <summary>
        /// The flag to check if approved or not
        /// </summary>
        [Required]
        public bool IsApproved { get; set; }
    }
}
