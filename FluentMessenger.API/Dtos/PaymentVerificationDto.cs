namespace FluentMessenger.API.Dtos {
    public class PaymentVerificationDto {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string TransactionReference { get; set; }
    }
}
