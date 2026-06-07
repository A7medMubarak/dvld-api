using DVLD.Contracts.Common;
using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.LocalLicenseApp;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using DVLD.DataAccess.Mapping;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Repositories
{
    public class LocalLicenseAppRepository : ILocalLicenseAppRepository
    {
        private readonly AppDbContext _context;

        public LocalLicenseAppRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LocalLicenseAppDto> CreateAsync(CreateLocalLicenseAppDto localLicenseAppDTO, CancellationToken ct = default)
        {
            // Create base application
            var applicationEntity = localLicenseAppDTO.ToApplicationEntity();
         
            // Create local license application
            var localLicenseEntity = new LocalDrivingLicenseApplication
            {
                ApplicationId = 0,  // EF Core will fix this upon save
                LicenseClassId = localLicenseAppDTO.LicenseClassId,
                Application = applicationEntity    // Link the parent to the child in memory
            };

            await _context.Applications.AddAsync(applicationEntity, ct);
            await _context.LocalDrivingLicenseApplications.AddAsync(localLicenseEntity, ct);
            
            await _context.SaveChangesAsync(ct);

            return localLicenseEntity.ToDto();
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await _context.LocalDrivingLicenseApplications
                .Where(l => l.LocalDrivingLicenseApplicationId == id)
                .ExecuteDeleteAsync(ct);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
            => await _context.LocalDrivingLicenseApplications
                .AnyAsync(l => l.LocalDrivingLicenseApplicationId == id, ct);

        public async Task<IReadOnlyList<LocalLicenseViewDto>> GetAllAsync(CancellationToken ct = default)
            => await _context.LocalDrivingLicenseApplications
                .AsNoTracking()
                .ProjectToView()
                .ToListAsync(ct);

        public async Task<PagedResult<LocalLicenseViewDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct = default)
            => await _context.LocalDrivingLicenseApplications
                .AsNoTracking()
                .ProjectToView()
                .ToPagedListAsync(paging, ct);

        public async Task<LocalLicenseAppDto?> GetByApplicationIdAsync(int applicationId, CancellationToken ct = default)
            => await _context.LocalDrivingLicenseApplications
                .AsNoTracking()
                .Where(l => l.ApplicationId == applicationId)
                .ProjectToDto()
                .FirstOrDefaultAsync(ct);

        public async Task<LocalLicenseAppDto?> GetByLocalLicenseAppIdAsync(int localLicenseAppId, CancellationToken ct = default)
            => await _context.LocalDrivingLicenseApplications
                .AsNoTracking()
                .Where(l => l.LocalDrivingLicenseApplicationId == localLicenseAppId)
                .ProjectToDto()
                .FirstOrDefaultAsync(ct);

        public async Task<byte> GetTestAttemptCountAsync(int localLicenseAppId, int testTypeId, CancellationToken ct = default)
        {
            var count = await _context.TestAppointments
                .AsNoTracking()
                .CountAsync(t =>
                t.LocalDrivingLicenseApplicationId == localLicenseAppId &&
                t.TestTypeId == testTypeId,
                ct);

            return (byte)count;
        }

        public async Task<bool> HasActiveTestAppointmentAsync(int localLicenseAppId, int testTypeId, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            return await _context.TestAppointments
                .AsNoTracking()
                .AnyAsync(t =>
                t.LocalDrivingLicenseApplicationId == localLicenseAppId &&
                t.TestTypeId == testTypeId &&
                t.Test == null &&
                t.AppointmentDate >= now
                , ct);
        }

        public async Task<bool> HasAttendedTestAsync(int localLicenseAppId, int testTypeId, CancellationToken ct = default)
            => await _context.TestAppointments
                .AsNoTracking()
                .AnyAsync(t =>
                t.LocalDrivingLicenseApplicationId == localLicenseAppId &&
                t.TestTypeId == testTypeId &&
                t.Test != null
                , ct);

        public async Task<bool> HasPassedTestAsync(int localLicenseAppId, int testTypeId, CancellationToken ct = default)
            => await _context.TestAppointments
                .AsNoTracking()
                .AnyAsync(t =>
                t.LocalDrivingLicenseApplicationId == localLicenseAppId &&
                t.TestTypeId == testTypeId &&
                t.Test != null &&
                t.Test.TestResult
                , ct);



        public async Task<LocalLicenseAppDto> UpdateAsync(int id, LocalLicenseAppDto localLicenseAppDTO, CancellationToken ct = default)
        {
            var entity = await _context.LocalDrivingLicenseApplications
                .Include(l => l.Application)
                .FirstOrDefaultAsync(l => l.LocalDrivingLicenseApplicationId == id, ct)
                    ?? throw new KeyNotFoundException($"Local license application with id {id} not found");

            if (entity.Application == null)
                throw new InvalidOperationException("Application not loaded");

            entity.Application.ApplicationStatus = (enApplicationStatus)localLicenseAppDTO.ApplicationStatus;
            entity.Application.LastStatusDate = localLicenseAppDTO.LastStatusDate;
            entity.Application.PaidFees = localLicenseAppDTO.PaidFees;
            entity.LicenseClassId = localLicenseAppDTO.LicenseClassId;

            await _context.SaveChangesAsync(ct);

            return entity.ToDto();

        }


    }
}
