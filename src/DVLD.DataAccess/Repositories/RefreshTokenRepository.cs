using DVLD.Contracts.Dtos.Auth;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(RefreshTokenDto dto, CancellationToken ct = default)
        {
            var entity = new RefreshToken
            {
                UserId = dto.UserId,
                Token = dto.Token,
                ExpiresAt = dto.ExpiresAt,
                CreatedAt = dto.CreatedAt
            };
            _context.RefreshTokens.Add(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<RefreshTokenDto?> GetByTokenAsync(string token, CancellationToken ct = default)
        {
            var entity = await _context.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(rt => rt.Token == token, ct);

            return entity == null ? null : ToDto(entity);
        }

        public async Task RevokeAsync(RefreshTokenDto dto, CancellationToken ct = default)
        {
            var entity = await _context.RefreshTokens.FindAsync(new object[] { dto.Id }, ct);
            if (entity != null)
            {
                entity.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task ReplaceAsync(RefreshTokenDto oldToken, string newToken, CancellationToken ct = default)
        {
            var entity = await _context.RefreshTokens.FindAsync(new object[] { oldToken.Id }, ct);
            if (entity != null)
            {
                entity.ReplacedByToken = newToken;
                entity.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task RevokeAllForUserAsync(int userId, CancellationToken ct = default)
        {
            await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(rt => rt.RevokedAt, DateTime.UtcNow),
                    ct);
        }

        private static RefreshTokenDto ToDto(RefreshToken entity) => new()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Token = entity.Token,
            ExpiresAt = entity.ExpiresAt,
            CreatedAt = entity.CreatedAt,
            RevokedAt = entity.RevokedAt,
            ReplacedByToken = entity.ReplacedByToken
        };
    }
}
