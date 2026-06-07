using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.DetainedLicense;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using DVLD.DataAccess.Mapping;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Repositories
{
    public class DetainedLicenseRepository : IDetainedLicenseRepository
    {
        private readonly AppDbContext _context;

        public DetainedLicenseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DetainedLicenseDto> CreateAsync(DetainedLicenseDto dto, CancellationToken cancellationToken)
        {
            DetainedLicense entity = dto.ToEntity();

            await _context.DetainedLicenses.AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return entity.ToDto();
        }

        public async Task<IReadOnlyList<DetainedLicenseViewDto>> GetAllAsync(CancellationToken cancellationToken)
            => await _context.DetainedLicenses
                .AsNoTracking()
                .OrderByDescending(d => d.DetainId)
                .ProjectToView()
                .ToListAsync(cancellationToken);

        public async Task<PagedResult<DetainedLicenseViewDto>> GetPagedAsync(PaginationParams paging, CancellationToken cancellationToken)
            => await _context.DetainedLicenses
                .AsNoTracking()
                .OrderByDescending(d => d.DetainId)
                .ProjectToView()
                .ToPagedListAsync(paging, cancellationToken);

        public async Task<DetainedLicenseDto?> GetByDetainIdAsync(int detainId, CancellationToken cancellationToken)
            => await _context.DetainedLicenses
                .AsNoTracking()
                .Where(d => d.DetainId == detainId)
                .ProjectToDto()
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<DetainedLicenseDto?> GetByLicenseIdAsync(int licenseId, CancellationToken cancellationToken)
            => await _context.DetainedLicenses
                .AsNoTracking()
                .Where(d => d.LicenseId == licenseId)
                .OrderByDescending(d => d.DetainId)
                .ProjectToDto()
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<bool> IsLicenseDetainedAsync(int licenseId, CancellationToken cancellationToken)
            => await _context.DetainedLicenses
                .AnyAsync(d => d.LicenseId == licenseId && !d.IsReleased, cancellationToken);

        public async Task<bool> ReleaseDetainedLicenseAsync(int detainId, int releasedByUserId, int releaseApplicationId, CancellationToken cancellationToken)
        {
            var entity = await _context.DetainedLicenses
                .FindAsync(detainId, cancellationToken);

            if (entity == null) return false;

            entity.IsReleased = true;
            entity.ReleaseDate = DateTime.UtcNow;
            entity.ReleaseApplicationId = releaseApplicationId;
            entity.ReleaseByUserId = releasedByUserId;

            await _context.SaveChangesAsync(cancellationToken);

            return true;

        }

    }
}
