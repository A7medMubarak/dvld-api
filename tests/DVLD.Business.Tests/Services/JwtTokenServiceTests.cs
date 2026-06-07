using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.Auth;
using DVLD.Contracts.Options;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace DVLD.Business.Tests.Services
{
    public class JwtTokenServiceTests
    {
        private readonly JwtSettings _settings;
        private readonly JwtTokenService _sut;

        public JwtTokenServiceTests()
        {
            _settings = new JwtSettings
            {
                SecretKey = "ThisIsASecretKeyForTestingPurposes12345678!",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpiryInMinutes = 60
            };
            var options = Substitute.For<IOptions<JwtSettings>>();
            options.Value.Returns(_settings);
            _sut = new JwtTokenService(options);
        }

        [Fact]
        public void GenerateToken_WithValidClaims_ReturnsJwtString()
        {
            var claims = new UserClaimsDto { UserId = 1, UserName = "testuser", Role = enRole.Admin };

            var result = _sut.GenerateToken(claims, CancellationToken.None);

            result.Should().NotBeNullOrEmpty();
            result.Split('.').Should().HaveCount(3);
        }

        [Fact]
        public void GenerateToken_WithDifferentUsers_ReturnsDifferentTokens()
        {
            var claims1 = new UserClaimsDto { UserId = 1, UserName = "user1", Role = enRole.Admin };
            var claims2 = new UserClaimsDto { UserId = 2, UserName = "user2", Role = enRole.Officer };

            var token1 = _sut.GenerateToken(claims1, CancellationToken.None);
            var token2 = _sut.GenerateToken(claims2, CancellationToken.None);

            token1.Should().NotBe(token2);
        }

        [Fact]
        public void GenerateToken_TokenContainsExpectedClaims()
        {
            var claims = new UserClaimsDto { UserId = 42, UserName = "jdoe", Role = enRole.Officer };

            var token = _sut.GenerateToken(claims, CancellationToken.None);

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.Claims.Should().Contain(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && c.Value == "42");
            jwt.Claims.Should().Contain(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "Officer");
            jwt.Claims.Should().Contain(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata" && c.Value == "jdoe");
            jwt.Claims.Should().Contain(c => c.Type == "jti");
        }

        [Fact]
        public void GenerateToken_TokenExpiresInFuture()
        {
            var claims = new UserClaimsDto { UserId = 1, UserName = "testuser", Role = enRole.Admin };

            var token = _sut.GenerateToken(claims, CancellationToken.None);

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.ValidTo.Should().BeAfter(DateTime.UtcNow.AddMinutes(55));
            jwt.ValidTo.Should().BeBefore(DateTime.UtcNow.AddMinutes(65));
        }

        [Fact]
        public void GenerateToken_WithCancellationToken_DoesNotThrow()
               {
            var claims = new UserClaimsDto { UserId = 1, UserName = "testuser", Role = enRole.Admin };

            var act = () => _sut.GenerateToken(claims, CancellationToken.None);

            act.Should().NotThrow();
        }
    }
}
