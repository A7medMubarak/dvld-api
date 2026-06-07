using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Test;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Mapping;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Repositories
{
    public class TestRepository : ITestRepository
    {
        private readonly AppDbContext _context;

        public TestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TestDto> CreateAsync(TestDto testDTO, CancellationToken ct = default)
        {
            var entity = testDTO.ToEntity();

            await _context.Tests.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);

            return entity.ToDto();
                
        }

        public async Task<IReadOnlyList<TestDto>> GetAllAsync(CancellationToken ct = default)
            => await _context.Tests
                .AsNoTracking()
                .ProjectToDto()
                .ToListAsync(ct);

        public async Task<PagedResult<TestDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct = default)
            => await _context.Tests
                .AsNoTracking()
                .OrderBy(t => t.TestId)
                .ProjectToDto()
                .ToPagedListAsync(paging, ct);

        public async Task<TestDto?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _context.Tests
                .AsNoTracking()
                .Where(t => t.TestId == id)
                .ProjectToDto()
                .FirstOrDefaultAsync(ct);

        public async Task<TestWithApplicantDto?> GetLatestAsync(int personId, int licenseClassId, int testTypeId, CancellationToken ct = default)       
            => await _context.Tests
                .AsNoTracking()
                .Where(t => t.TestAppointment.TestTypeId == testTypeId &&
                            t.TestAppointment.LocalDrivingLicenseApplication.LicenseClassId == licenseClassId &&
                            t.TestAppointment.LocalDrivingLicenseApplication.Application.ApplicantPersonId == personId)
                .OrderByDescending(t => t.TestAppointment.AppointmentDate)
                .ProjectToTestWithApplicantDto()
                .FirstOrDefaultAsync(ct);


        public async Task<int> GetPassedCountAsync(int localDrivingLicenseAppId, CancellationToken ct = default)
            => await _context.Tests
                .AsNoTracking()
                .CountAsync(t => 
                    t.TestAppointment.LocalDrivingLicenseApplicationId == localDrivingLicenseAppId &&
                    t.TestResult, ct);

        public async Task<TestDto> UpdateAsync(int id, TestDto testDTO, CancellationToken ct = default)
        {
            var entity = await _context.Tests.FindAsync(id, ct)
                ?? throw new KeyNotFoundException($"Test with id {id} not found.");

            entity.TestResult = testDTO.TestResult;
            entity.Notes = testDTO.Notes;

            await _context.SaveChangesAsync(ct);

            return entity.ToDto();
        }
    }
}
