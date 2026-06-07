using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.License;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Mapping;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Repositories
{
    public class LicenseRepository : ILicenseRepository
    {
        private readonly AppDbContext _context;

        public LicenseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LicenseDto> CreateAsync(LicenseDto licenseDTO, CancellationToken ct = default)
        {
            var entity = licenseDTO.ToEntity();

            await _context.Licenses.AddAsync(entity, ct);
           
            await _context.SaveChangesAsync(ct);
           
            return entity.ToDto();
        }
        

        public async Task<bool> DeactivateAsync(int licenseId, CancellationToken ct = default)
        {
            var affected = await _context.Licenses
                .Where(l => l.LicenseId == licenseId)
                .ExecuteUpdateAsync(setters => setters
                .SetProperty(l => l.IsActive, false), 
                ct);
                
            return affected > 0;
        }

        public async Task<bool> ExistsAsync(int licenseId, CancellationToken ct = default)
            => await _context.Licenses
                .AnyAsync(l => l.LicenseId == licenseId, ct);



        public async Task<LicenseDto?> GetActiveLicenseForPersonAsync(int personId, int licenseClassId, CancellationToken ct = default)
            => await _context.Licenses
                .AsNoTracking()
                .Include(l => l.Driver)
                .Where(l => l.LicenseClassId == licenseClassId && l.Driver.PersonId == personId && l.IsActive)
                .ProjectToDto()
                .FirstOrDefaultAsync(ct);

        public async Task<IReadOnlyList<LicenseDto>> GetAllAsync(CancellationToken ct = default)
            => await _context.Licenses
                .AsNoTracking()
                .ProjectToDto()
                .ToListAsync(ct);

        public async Task<PagedResult<LicenseDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct = default)
            => await _context.Licenses
                .AsNoTracking()
                .ProjectToDto()
                .ToPagedListAsync(paging, ct);

        public async Task<LicenseDto?> GetByIdAsync(int licenseId, CancellationToken ct = default)
            => await _context.Licenses
                .AsNoTracking()
                .Where(l => l.LicenseId == licenseId)
                .ProjectToDto()
                .FirstOrDefaultAsync(ct);

        public async Task<IReadOnlyList<DriverLicensesDto>> GetDriverLicensesAsync(int driverId, CancellationToken ct = default)
            => await _context.Licenses
                .AsNoTracking()
                .Where(l => l.DriverId == driverId)
                .ProjectToDriverLicensesDto()
                .ToListAsync(ct);

        public async Task<PagedResult<DriverLicensesDto>> GetPagedDriverLicensesAsync(int driverId, PaginationParams paging, CancellationToken ct = default)
            => await _context.Licenses
                .AsNoTracking()
                .Where(l => l.DriverId == driverId)
                .ProjectToDriverLicensesDto()
                .ToPagedListAsync(paging, ct);

        public async Task<LicenseDto> UpdateAsync(int licenseId, LicenseDto licenseDTO, CancellationToken ct = default)
        {
            var entity = await _context.Licenses.FindAsync(licenseId,ct)
                ?? throw new KeyNotFoundException($"License with id {licenseId} not found");
          
            entity.IssueDate = licenseDTO.IssueDate;
            entity.ExpirationDate = licenseDTO.ExpirationDate;
            entity.IssueReason = licenseDTO.IssueReason;
            entity.Notes = licenseDTO.Notes;
            entity.PaidFees = licenseDTO.PaidFees;
            entity.IsActive = licenseDTO.IsActive;

            await _context.SaveChangesAsync(ct);

            return entity.ToDto();

        }

      
    }
}
