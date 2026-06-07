using DVLD.Contracts.Dtos.Auth;
using DVLD.Contracts.Options;
using DVLD.Business.Interfaces.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DVLD.Business.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _settings;

        public JwtTokenService(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
        }

        public string GenerateToken(UserClaimsDto userClaims, CancellationToken ct)
        {
            // 1. Define what goes INSIDE the token (claims)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userClaims.UserId.ToString()),
                new Claim(ClaimTypes.UserData, userClaims.UserName),
                new Claim(ClaimTypes.Role, userClaims.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // 2. Create signing key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.SecretKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. Build the token
            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryInMinutes),
                signingCredentials: credentials
                );

            // 4. Serialize to string
           return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
