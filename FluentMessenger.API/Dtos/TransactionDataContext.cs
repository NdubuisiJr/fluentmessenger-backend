using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluentMessenger.API.Dtos {
    public class TransactionDataContext {
        public bool? Status { get; set; }
        public string Message { get; set; }
        public List<TransactionData> Data { get; set; }
        public TransactionContextMetaData Meta { get; set; }
    }
}
