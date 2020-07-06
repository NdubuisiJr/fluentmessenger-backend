using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluentMessenger.API.Dtos {
    public class TransactionContextMetaData {
        public string Total { get; set; }
        public string Total_Volume { get; set; }
        public string Skipped { get; set; }
        public string PerPage { get; set; }
        public string Page { get; set; }
        public string PageCount { get; set; }
    }
}
