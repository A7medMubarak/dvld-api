using DVLD.Business.Interfaces.Services;
using DVLD.Contracts.Requests.Auth;
using DVLD.Contracts.Response.Auth;
using DVLD.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.Api.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly IAuthService _authService;
        private readonly ISecurityAuditService _audit;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authService = Substitute.For<IAuthService>();
            _audit = Substitute.For<ISecurityAuditService>();
            _controller = new AuthController(_authService, _audit);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithAuthResponse()
        {
            var request = new LoginRequest { UserName = "admin", Password = "pass" };
            var expected = new AuthResponse { Token = "jwt", UserName = "admin", Role = Contracts.Common.Enums.enRole.Admin };
            _authService.LoginAsync(request, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.Login(request, CancellationToken.None);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(expected);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var request = new LoginRequest { UserName = "admin", Password = "wrong" };
            _authService.LoginAsync(request, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.Login(request, CancellationToken.None);

            result.Should().BeOfType<UnauthorizedObjectResult>();
            _audit.Received(1).LogFailedLogin("admin");
        }

        [Fact]
        public async Task Refresh_ValidToken_ReturnsOkWithAuthResponse()
        {
            var request = new RefreshTokenRequest { Token = "valid-refresh" };
            var expected = new AuthResponse { Token = "new-jwt", UserName = "admin", Role = Contracts.Common.Enums.enRole.Admin };
            _authService.RefreshTokenAsync(request.Token, Arg.Any<CancellationToken>()).Returns(expected);

            var result = await _controller.Refresh(request, CancellationToken.None);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(expected);
        }

        [Fact]
        public async Task Refresh_InvalidToken_ReturnsUnauthorized()
        {
            var request = new RefreshTokenRequest { Token = "invalid" };
            _authService.RefreshTokenAsync(request.Token, Arg.Any<CancellationToken>()).ReturnsNull();

            var result = await _controller.Refresh(request, CancellationToken.None);

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Logout_ReturnsOk()
        {
            var request = new LogoutRequest { RefreshToken = "token" };

            var result = await _controller.Logout(request, CancellationToken.None);

            result.Should().BeOfType<OkResult>();
            await _authService.Received(1).LogoutAsync(request.RefreshToken, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task LogoutAll_ReturnsOk()
        {
            var result = await _controller.LogoutAll(CancellationToken.None);

            result.Should().BeOfType<OkResult>();
            await _authService.Received(1).LogoutAllAsync(Arg.Any<CancellationToken>());
        }
    }
}
