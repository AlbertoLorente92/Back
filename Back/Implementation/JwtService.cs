using Back.Interfaces;
using Back.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Back.Implementation
{
    public class JwtService : IJwtService
    {
        private readonly IUserService _userService;

        private const string SECRET_KEY = "TuClaveSecretaMuySegura123456789!";
        private const string ISSUER = "https://tuaplicacion.com";
        private const string AUDIENCE = "https://tucliente.com";
        private const double EXPIRE_IN_MINUTES = 120d;
        public JwtService(IUserService userService) 
        { 
            _userService = userService;
        }
        public string? GetToken(GetTokenRequest request)
        {
            var user = _userService.GetUserByEmail(request.Email);
            if (user == null)
            {
                return null;
            }

            if (!_userService.IsUserLoginCorrect(request.Email, request.Password))
            {
                return null;
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.GivenName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Sub, request.Email),
                new Claim(JwtRegisteredClaimNames.Jti, user.Guid.ToString()),
                new Claim("rol", "admin")
            };

            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(EXPIRE_IN_MINUTES),
                Issuer = ISSUER,
                Audience = AUDIENCE,
                SigningCredentials = creds
            };

            
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
