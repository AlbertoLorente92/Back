using Back.Models;

namespace Back.Interfaces
{
    public interface IPasswordHashService
    {
        public HashPasswordResponse HashPassword(string plainPassword);
        public bool VerifyPassword(string password, string hashedPassword, string salt);
    }
}
