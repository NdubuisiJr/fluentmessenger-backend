using System;

namespace FluentMessenger.API.Dtos {
    public class TransactionData {
        public int Id { get; set; }
        public string Domain { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public decimal? Amount { get; set; }
        public string Message { get; set; }
        public string GateWay_Response { get; set; }
        public DateTime? Paid_At { get; set; }
        public DateTime? Created_At { get; set; }
        public string Channel { get; set; }
        public string Currency { get; set; }
        public string Ip_Address { get; set; }
        public decimal? Fees { get; set; }
        public string Fees_Split { get; set; }
        public object Customer { get; set; }
        public object MetaData { get; set; }
        public object Log { get; set; }
        public object Authorization { get; set; }
        public object Plan { get; set; }
        public object Subaccount { get; set; }
        public string Order_Id { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public decimal? Requested_Amount { get; set; }
    }
}
