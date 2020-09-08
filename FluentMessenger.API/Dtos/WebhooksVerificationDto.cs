using System;

namespace FluentMessenger.API.Dtos {
    public class WebhooksVerificationDto {
        public string Event { get; set; }
        public Data Data { get; set; }
    }

    public class Data {
        public double Id { get; set; }
        public string Domain { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; }
        public string GateWay_Response { get; set; }
        public DateTime? Paid_At { get; set; }
        public DateTime? Created_At { get; set; }
        public string Channel { get; set; }
        public string Currency { get; set; }
        public string Ip_Address { get; set; }
        public object MetaData { get; set; }
        public Log Log { get; set; }
        public object Fees { get; set; }
	public object Fees_Split{get;set;}
        public Customer Customer { get; set; }
        public Authorization Authorization { get; set; }
        public object Plan { get; set; }
	public object Subaccount{get;set;}
	public object split {get;set;}
	public object Order_id{get;set;}
	public object PaidAt{get;set;}
	public object Requested_amount{get;set;}

    }
    public class Log {
        public double Time_Spent { get; set; }
	public double Start_Time{get;set;}
        public int Attempts { get; set; }
        public string Authentication { get; set; }
        public int Errors { get; set; }
        public bool Success { get; set; }
        public bool Mobile { get; set; }
        public object input { get; set; }
        public object Channel { get; set; }
        public object History { get; set; }
    }
    public class Customer {
        public double Id { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public string Customer_Code { get; set; }
        public object Phone { get; set; }
        public object MetaData { get; set; }
        public string Risk_Action { get; set; }
    }
    public class Authorization {
        public string AuthorizationCode { get; set; }
        public string Bin { get; set; }
        public string Last4 { get; set; }
        public string Exp_Month { get; set; }
        public string Exp_Year { get; set; }
        public string Card_Type { get; set; }
        public string Bank { get; set; }
        public string Country_Code { get; set; }
        public string Brand { get; set; }
    }
}
