using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluentMessenger.API.Dtos {
    public class ContactMessageForCreationDto {
        public IEnumerable<int> ContactKeys { get; set; }
        public IEnumerable<int> MessageKeys { get; set; }
        public bool IsReceived { get; set; }

        internal bool IsConsistent() {
            return ContactKeys.Count() == MessageKeys.Count();
        }
    }
}
