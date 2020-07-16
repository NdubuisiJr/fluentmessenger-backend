namespace FluentMessenger.API.Dtos {
    /// <summary>
    /// The input object for confirming payment
    /// </summary>
    public class PaymentVerificationDto {
        /// <summary>
        /// The user's Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The user's email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The transaction code generated during payment
        /// </summary>
        public string TransactionReference { get; set; }
    }
}
