using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluentMessenger.API.Dtos {
    /// <summary>
    /// The object that holds the query parameters for getting all contact Messages
    /// </summary>
    public class ContactMessageQuery {
        /// <summary>
        /// The messageId
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// The contact Id
        /// </summary>
        public int ContactId { get; set; }

        /// <summary>
        /// Checks to ensure that only contactId or messageId is supplied
        /// </summary>
        /// <returns></returns>
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
