using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluentMessenger.API.Dtos {
    /// <summary>
    /// The object that creates a contactMessage relationship
    /// </summary>
    public class ContactMessageForCreationDto {
        /// <summary>
        /// Collection of contact keys
        /// </summary>
        public IEnumerable<int> ContactKeys { get; set; }

        /// <summary>
        /// collection of message keys
        /// </summary>
        public IEnumerable<int> MessageKeys { get; set; }

        /// <summary>
        /// Flag to differentiate Received and missed
        /// </summary>
        public bool IsReceived { get; set; }

        /// <summary>
        /// Returns a check to see if the two collections have equal lengths
        /// </summary>
        /// <returns></returns>
        internal bool IsConsistent() {
            return ContactKeys.Count() == MessageKeys.Count();
        }
    }
}
