using DVLD.Contracts.Dtos.Auth;
using DVLD.Contracts.Options;
using DVLD.Contracts.Requests.Auth;
using DVLD.Contracts.Response.Auth;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace DVLD.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtTokenService _jwtService;
        private readonly IPasswordService _passwordService;
        private readonly ICurrentUserService _currentUser;
        private readonly JwtSettings _settings;

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtTokenService jwtService,
            IPasswordService passwordService,
            ICurrentUserService currentUser,
            IOptions<JwtSettings> settings)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtService = jwtService;
            _passwordService = passwordService;
            _currentUser = currentUser;
            _settings = settings.Value;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var userCredentials = await _userRepository.GetCredentialsByUsernameAsync(request.UserName, ct);
            if (userCredentials == null || !userCredentials.IsActive)
                return null;

            if (!_passwordService.Verify(request.Password, userCredentials.PasswordHash))
                return null;

            var userClaims = await _userRepository.GetClaimsByUsernameAsync(request.UserName, ct);
            if (userClaims == null)
                return null;

            var token = _jwtService.GenerateToken(userClaims, ct);
            var (rawRefresh, refreshHash) = GenerateRefreshToken();

            var refreshTokenDto = new RefreshTokenDto
            {
                UserId = userCredentials.UserId,
                Token = refreshHash,
                ExpiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpiryDays),
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.CreateAsync(refreshTokenDto, ct);

            return new AuthResponse
            {
                Token = token,
                UserName = userClaims.UserName,
                Role = userClaims.Role,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpiryInMinutes),
                RefreshToken = rawRefresh
            };
        }

        public async Task<AuthResponse?> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        {
            var tokenHash = ComputeHash(refreshToken);
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(tokenHash, ct);
            if (storedToken == null)
                return null;

            if (storedToken.RevokedAt.HasValue || !string.IsNullOrEmpty(storedToken.ReplacedByToken))
                return null;

            if (storedToken.ExpiresAt < DateTime.UtcNow)
            {
                await _refreshTokenRepository.RevokeAsync(storedToken, ct);
                return null;
            }

            var user = await _userRepository.GetByUserIdAsync(storedToken.UserId, ct);
            if (user == null)
                return null;

            var userClaims = await _userRepository.GetClaimsByUsernameAsync(user.UserName, ct);
            if (userClaims == null)
                return null;

            var newAccessToken = _jwtService.GenerateToken(userClaims, ct);
            var (newRaw, newHash) = GenerateRefreshToken();

            await _refreshTokenRepository.ReplaceAsync(storedToken, newHash, ct);

            var newRefreshTokenDto = new RefreshTokenDto
            {
                UserId = storedToken.UserId,
                Token = newHash,
                ExpiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpiryDays),
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.CreateAsync(newRefreshTokenDto, ct);

            return new AuthResponse
            {
                Token = newAccessToken,
                UserName = userClaims.UserName,
                Role = userClaims.Role,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpiryInMinutes),
                RefreshToken = newRaw
            };
        }

        public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
        {
            var tokenHash = ComputeHash(refreshToken);
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(tokenHash, ct);
            if (storedToken != null && !storedToken.RevokedAt.HasValue)
            {
                await _refreshTokenRepository.RevokeAsync(storedToken, ct);
            }
        }

        public async Task LogoutAllAsync(CancellationToken ct = default)
        {
            await _refreshTokenRepository.RevokeAllForUserAsync(_currentUser.UserId, ct);
        }

        private static (string token, string hash) GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var raw = Convert.ToBase64String(randomBytes);
            var hash = ComputeHash(raw);
            return (raw, hash);
        }

        private static string ComputeHash(string value)
            => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    }
}
