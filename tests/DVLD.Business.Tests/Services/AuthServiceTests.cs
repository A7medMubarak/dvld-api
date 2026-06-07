using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.Auth;
using DVLD.Contracts.Dtos.User;
using DVLD.Contracts.Options;
using DVLD.Contracts.Requests.Auth;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DVLD.Business.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly IUserRepository _userRepo;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IJwtTokenService _jwtService;
        private readonly IPasswordService _passwordService;
        private readonly ICurrentUserService _currentUser;
        private readonly JwtSettings _settings;
        private readonly AuthService _sut;

        public AuthServiceTests()
        {
            _userRepo = Substitute.For<IUserRepository>();
            _refreshTokenRepo = Substitute.For<IRefreshTokenRepository>();
            _jwtService = Substitute.For<IJwtTokenService>();
            _passwordService = Substitute.For<IPasswordService>();
            _passwordService.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(callInfo =>
            {
                var password = callInfo.ArgAt<string>(0);
                var hash = callInfo.ArgAt<string>(1);
                return BCrypt.Net.BCrypt.Verify(password, hash);
            });
            _currentUser = Substitute.For<ICurrentUserService>();
            _settings = new JwtSettings
            {
                SecretKey = "ThisIsATestSecretKeyForTestingPurposesOnly123!",
                Issuer = "Test",
                Audience = "Test",
                ExpiryInMinutes = 60,
                RefreshTokenExpiryDays = 7
            };
            var options = Substitute.For<IOptions<JwtSettings>>();
            options.Value.Returns(_settings);
            _sut = new AuthService(_userRepo, _refreshTokenRepo, _jwtService, _passwordService, _currentUser, options);
        }

        private static UserCredentialsDto CreateSampleCredentials() => new()
        {
            UserId = 1,
            UserName = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
            IsActive = true
        };

        private static UserClaimsDto CreateSampleClaims() => new()
        {
            UserId = 1,
            UserName = "testuser",
            Role = enRole.Admin
        };

        private static RefreshTokenDto CreateSampleRefreshToken() => new()
        {
            Id = 1,
            UserId = 1,
            Token = BCrypt.Net.BCrypt.HashPassword("sometoken"),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
        {
            var request = new LoginRequest { UserName = "testuser", Password = "Test@123" };
            var credentials = CreateSampleCredentials();
            var claims = CreateSampleClaims();

            _userRepo.GetCredentialsByUsernameAsync("testuser", Arg.Any<CancellationToken>()).Returns(credentials);
            _userRepo.GetClaimsByUsernameAsync("testuser", Arg.Any<CancellationToken>()).Returns(claims);
            _jwtService.GenerateToken(claims, Arg.Any<CancellationToken>()).Returns("jwt-token");

            var result = await _sut.LoginAsync(request, CancellationToken.None);

            result.Should().NotBeNull();
            result!.Token.Should().Be("jwt-token");
            result.UserName.Should().Be("testuser");
            result.Role.Should().Be(enRole.Admin);
            result.RefreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ReturnsNull()
        {
            var request = new LoginRequest { UserName = "testuser", Password = "WrongPass@1" };
            var credentials = CreateSampleCredentials();
            _userRepo.GetCredentialsByUsernameAsync("testuser", Arg.Any<CancellationToken>()).Returns(credentials);

            var result = await _sut.LoginAsync(request, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task LoginAsync_WhenUserNotFound_ReturnsNull()
        {
            var request = new LoginRequest { UserName = "unknown", Password = "Test@123" };
            _userRepo.GetCredentialsByUsernameAsync("unknown", Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.LoginAsync(request, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task LoginAsync_WhenUserInactive_ReturnsNull()
        {
            var request = new LoginRequest { UserName = "inactive", Password = "Test@123" };
            var credentials = new UserCredentialsDto { UserId = 2, UserName = "inactive", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"), IsActive = false };
            _userRepo.GetCredentialsByUsernameAsync("inactive", Arg.Any<CancellationToken>()).Returns(credentials);

            var result = await _sut.LoginAsync(request, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task LoginAsync_WhenClaimsNull_ReturnsNull()
        {
            var request = new LoginRequest { UserName = "testuser", Password = "Test@123" };
            var credentials = CreateSampleCredentials();
            _userRepo.GetCredentialsByUsernameAsync("testuser", Arg.Any<CancellationToken>()).Returns(credentials);
            _userRepo.GetClaimsByUsernameAsync("testuser", Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.LoginAsync(request, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task LoginAsync_CreatesRefreshToken()
        {
            var request = new LoginRequest { UserName = "testuser", Password = "Test@123" };
            var credentials = CreateSampleCredentials();
            var claims = CreateSampleClaims();

            _userRepo.GetCredentialsByUsernameAsync("testuser", Arg.Any<CancellationToken>()).Returns(credentials);
            _userRepo.GetClaimsByUsernameAsync("testuser", Arg.Any<CancellationToken>()).Returns(claims);
            _jwtService.GenerateToken(claims, Arg.Any<CancellationToken>()).Returns("jwt-token");

            var result = await _sut.LoginAsync(request, CancellationToken.None);

            await _refreshTokenRepo.Received(1).CreateAsync(Arg.Is<RefreshTokenDto>(t => t.UserId == 1), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task RefreshTokenAsync_WithValidToken_ReturnsAuthResponse()
        {
            var rawRefresh = "valid-raw-token";
            var storedToken = CreateSampleRefreshToken();
            var userClaims = CreateSampleClaims();

            _refreshTokenRepo.GetByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(storedToken);
            _userRepo.GetByUserIdAsync(1, Arg.Any<CancellationToken>()).Returns(new UserDto(1, 1, "testuser", true, enRole.Admin));
            _userRepo.GetClaimsByUsernameAsync("testuser", Arg.Any<CancellationToken>()).Returns(userClaims);
            _jwtService.GenerateToken(userClaims, Arg.Any<CancellationToken>()).Returns("new-jwt-token");

            var result = await _sut.RefreshTokenAsync(rawRefresh, CancellationToken.None);

            result.Should().NotBeNull();
            result!.Token.Should().Be("new-jwt-token");
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenTokenNotFound_ReturnsNull()
        {
            _refreshTokenRepo.GetByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _sut.RefreshTokenAsync("invalid-token", CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenTokenRevoked_ReturnsNull()
        {
            var storedToken = CreateSampleRefreshToken();
            storedToken.RevokedAt = DateTime.UtcNow;
            _refreshTokenRepo.GetByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(storedToken);

            var result = await _sut.RefreshTokenAsync("revoked-token", CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenTokenReplaced_ReturnsNull()
        {
            var storedToken = CreateSampleRefreshToken();
            storedToken.ReplacedByToken = "new-token-hash";
            _refreshTokenRepo.GetByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(storedToken);

            var result = await _sut.RefreshTokenAsync("replaced-token", CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenTokenExpired_ReturnsNull()
        {
            var storedToken = CreateSampleRefreshToken();
            storedToken.ExpiresAt = DateTime.UtcNow.AddDays(-1);
            _refreshTokenRepo.GetByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(storedToken);

            var result = await _sut.RefreshTokenAsync("expired-token", CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenTokenExpired_RevokesIt()
        {
            var storedToken = CreateSampleRefreshToken();
            storedToken.ExpiresAt = DateTime.UtcNow.AddDays(-1);
            _refreshTokenRepo.GetByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(storedToken);

            await _sut.RefreshTokenAsync("expired-token", CancellationToken.None);

            await _refreshTokenRepo.Received(1).RevokeAsync(storedToken, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task LogoutAsync_WithValidToken_RevokesIt()
        {
            var storedToken = CreateSampleRefreshToken();
            _refreshTokenRepo.GetByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(storedToken);

            await _sut.LogoutAsync("valid-token", CancellationToken.None);

            await _refreshTokenRepo.Received(1).RevokeAsync(storedToken, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task LogoutAsync_WhenTokenRevoked_DoesNotRevokeAgain()
        {
            var storedToken = CreateSampleRefreshToken();
            storedToken.RevokedAt = DateTime.UtcNow;
            _refreshTokenRepo.GetByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(storedToken);

            await _sut.LogoutAsync("revoked-token", CancellationToken.None);

            await _refreshTokenRepo.DidNotReceive().RevokeAsync(Arg.Any<RefreshTokenDto>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task LogoutAllAsync_RevokesAllForUser()
        {
            _currentUser.UserId.Returns(1);

            await _sut.LogoutAllAsync(CancellationToken.None);

            await _refreshTokenRepo.Received(1).RevokeAllForUserAsync(1, Arg.Any<CancellationToken>());
        }
    }
}
