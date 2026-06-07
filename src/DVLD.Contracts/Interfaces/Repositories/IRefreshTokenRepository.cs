using DVLD.Contracts.Dtos.Auth;

namespace DVLD.Contracts.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task CreateAsync(RefreshTokenDto token, CancellationToken ct = default);
        Task<RefreshTokenDto?> GetByTokenAsync(string token, CancellationToken ct = default);
        Task RevokeAsync(RefreshTokenDto token, CancellationToken ct = default);
        Task ReplaceAsync(RefreshTokenDto oldToken, string newToken, CancellationToken ct = default);
        Task RevokeAllForUserAsync(int userId, CancellationToken ct = default);
    }
}
