namespace FluentMessenger.API.Dtos {
    public class UserDto {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string OtherNames { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal SMSCredit { get; set; }
        public int VerificationCode { get; set; }
        public string Token { get; set; }
    }
}
