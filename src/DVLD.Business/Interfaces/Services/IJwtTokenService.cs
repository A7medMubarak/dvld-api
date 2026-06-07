using DVLD.Contracts.Dtos.Auth;

namespace DVLD.Business.Interfaces.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(UserClaimsDto user, CancellationToken ct = default);
    }
}
