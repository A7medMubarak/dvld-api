using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.InternationalLicense;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Mapping;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Repositories
{
    public class InternationalLicenseRepository : IInternationalLicenseRepository
    {
        private readonly AppDbContext _context;

        public InternationalLicenseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<InternationalLicenseDto> CreateAsync(InternationalLicenseDto internationalLicenseDto, CancellationToken ct = default)
        {
            var entity = internationalLicenseDto.ToEntity();

            await _context.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);

            return entity.ToDto();

        }

        public async Task<InternationalLicenseDto?> GetActiveByDriverIdAsync(int driverId, CancellationToken ct = default)
            => await _context.InternationalLicenses
                .AsNoTracking()
                .Where(i => i.DriverId == driverId && i.IsActive)
                .ProjectToDto()
                .FirstOrDefaultAsync(ct);
            
        public async Task<IReadOnlyList<InternationalLicenseDto>> GetAllAsync(CancellationToken ct = default)        
            => await _context.InternationalLicenses
                .AsNoTracking()
                .ProjectToDto()
                .ToListAsync(ct);

        public async Task<PagedResult<InternationalLicenseDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct = default)
            => await _context.InternationalLicenses
                .AsNoTracking()
                .ProjectToDto()
                .OrderBy(i => i.InternationalLicenseId)
                .ToPagedListAsync(paging, ct);
        
        public async Task<IReadOnlyList<InternationalLicenseDto>> GetAllDriverLicensesAsync(int driverId, CancellationToken ct = default)
            => await _context.InternationalLicenses
                .AsNoTracking()
                .Where(i => i.DriverId == driverId)
                .ProjectToDto()
                .ToListAsync(ct);

        public async Task<PagedResult<InternationalLicenseDto>> GetPagedDriverLicensesAsync(int driverId, PaginationParams paging, CancellationToken ct = default)
            => await _context.InternationalLicenses
                .AsNoTracking()
                .Where(i => i.DriverId == driverId)
                .ProjectToDto()
                .OrderBy(i => i.InternationalLicenseId)
                .ToPagedListAsync(paging, ct);

        public async Task<InternationalLicenseDto?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _context.InternationalLicenses
                .AsNoTracking()
                .Where(i => i.InternationalLicenseId == id)
                .ProjectToDto()
                .FirstOrDefaultAsync(ct);

        public async Task<InternationalLicenseDto> UpdateAsync(int id, InternationalLicenseDto internationalLicenseDto, CancellationToken ct = default)
        {
            var entity = await _context.InternationalLicenses.FindAsync(id, ct)
                ?? throw new KeyNotFoundException($"International license with id {id} not found");

            entity.IssueDate = internationalLicenseDto.IssueDate;
            entity.ExpirationDate = internationalLicenseDto.ExpirationDate;
            entity.IsActive = internationalLicenseDto.IsActive;

            await _context.SaveChangesAsync(ct);
            return entity.ToDto();
        }
    }
}
