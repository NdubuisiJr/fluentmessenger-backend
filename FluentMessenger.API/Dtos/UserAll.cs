using System.Collections.Generic;

namespace FluentMessenger.API.Dtos {
    /// <summary>
    /// The structure of a returned user
    /// </summary>
    public class UserAll {
        /// <summary>
        /// The user's Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The user's Surname
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// The user's other names
        /// </summary>
        public string OtherNames { get; set; }

        /// <summary>
        /// The user's email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The user's phone number. In the nigerian format. without national code
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// The amount of SMS unit the user currently has
        /// </summary>
        public decimal SMSCredit { get; set; }

        /// <summary>
        /// The code used for verifying the user
        /// </summary>
        public int VerificationCode { get; set; }

        /// <summary>
        /// The authentication token issued to the user
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Sender Id
        /// </summary>
        public SenderIdDto Sender { get; set; }

        /// <summary>
        /// The user's notifications
        /// </summary>
        public IEnumerable<NotificationDto> Notifications { get; set; }

        /// <summary>
        /// Groups
        /// </summary>
        public virtual IEnumerable<GroupDto> Groups { get; set; }

        /// <summary>
        /// Message Templates
        /// </summary>
        public virtual IEnumerable<TemplateDto> MessageTemplates { get; set; }
    }
}
