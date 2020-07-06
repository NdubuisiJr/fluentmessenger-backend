using FluentMessenger.API.Entities;

namespace FluentMessenger.API.Interfaces {
    public interface ISecurityService {
        string GenerateJwtToken(User user);
        public (string, string) HashPassword(string password, int saltSize = 10);
        bool VerifyPassword(string password, string hashedPassword, string saltString);
        
        const string SPLITER  = "FLUENT";
    }
}
