using FluentMessenger.API.Dtos;

namespace FluentMessenger.API.Interfaces {
    public interface ISecurityService {
        string GenerateJwtToken(UserDto user);
        public (string, string) HashPassword(string password, int saltSize = 10);
        bool VerifyPassword(string password, string hashedPassword, string saltString);
        
        const string SPLITER  = "FLUENT";
    }
}
