using DVLD.Contracts.Requests.Auth;
using DVLD.Contracts.Response.Auth;
using DVLD.Business.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DVLD.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ISecurityAuditService _audit;

        public AuthController(IAuthService authService, ISecurityAuditService audit)
        {
            _authService = authService;
            _audit = audit;
        }

        [HttpPost("login")]
        [EnableRateLimiting("Auth")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var result = await _authService.LoginAsync(request, ct);

            if (result is null)
            {
                _audit.LogFailedLogin(request.UserName);
                return Unauthorized("Invalid credentials.");
            }

            return Ok(result);
        }

        [HttpPost("refresh")]
        [EnableRateLimiting("Auth")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
        {
            var result = await _authService.RefreshTokenAsync(request.Token, ct);

            return result is null
                ? Unauthorized("Invalid or expired refresh token.")
                : Ok(result);
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken ct)
        {
            await _authService.LogoutAsync(request.RefreshToken, ct);
            return Ok();
        }

        [Authorize]
        [HttpPost("logout-all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LogoutAll(CancellationToken ct)
        {
            await _authService.LogoutAllAsync(ct);
            return Ok();
        }
    }
}
