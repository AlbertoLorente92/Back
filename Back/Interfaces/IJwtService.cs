using Back.Models;

namespace Back.Interfaces
{
    public interface IJwtService
    {
        public string? GetToken(GetTokenRequest request);
    }
}
