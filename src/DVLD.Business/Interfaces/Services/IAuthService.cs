using DVLD.Contracts.Requests.Auth;
using DVLD.Contracts.Response.Auth;

namespace DVLD.Business.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default);
        Task<AuthResponse?> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
        Task LogoutAsync(string refreshToken, CancellationToken ct = default);
        Task LogoutAllAsync(CancellationToken ct = default);
    }
}
