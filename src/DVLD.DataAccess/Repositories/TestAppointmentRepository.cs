using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Test;
using DVLD.Contracts.Dtos.TestAppointment;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Mapping;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Repositories
{
    public class TestAppointmentRepository : ITestAppointmentRepository
    {
        private readonly AppDbContext _context;

        public TestAppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TestAppointmentDto> CreateAsync(TestAppointmentDto testAppointmentDTO, CancellationToken ct = default)
        {
            var entity = testAppointmentDTO.ToEntity();

            await _context.TestAppointments.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);

            return entity.ToDto();
        }

        public async Task<IReadOnlyList<TestAppointmentViewDto>> GetAllAsync(CancellationToken ct = default)
            => await _context.TestAppointments
                .AsNoTracking()
                .ProjectToView()
                .ToListAsync(ct);

        public async Task<PagedResult<TestAppointmentViewDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct = default)
            => await _context.TestAppointments
                .AsNoTracking()
                .ProjectToView()
                .OrderBy(t => t.TestAppointmentId)
                .ToPagedListAsync(paging, ct);

        public async Task<IReadOnlyList<TestAppointmentByTestTypeDto>> GetAllByTestTypeAsync(int localLicenseAppId, int testTypeId, CancellationToken ct = default)
            => await _context.TestAppointments
               .AsNoTracking()
               .Where(t => t.TestTypeId == testTypeId && t.LocalDrivingLicenseApplicationId == localLicenseAppId)
               .Select(t => new TestAppointmentByTestTypeDto
               {
                   TestAppointmentId = t.TestAppointmentId,
                   AppointmentDate = t.AppointmentDate,
                   PaidFees = t.PaidFees,
                   IsLocked = t.IsLocked 
               })
               .ToListAsync(ct);

        public async Task<PagedResult<TestAppointmentByTestTypeDto>> GetPagedByTestTypeAsync(
            int localLicenseAppId, int testTypeId, PaginationParams paging, CancellationToken ct = default)
            => await _context.TestAppointments
               .AsNoTracking()
               .Where(t => t.TestTypeId == testTypeId && t.LocalDrivingLicenseApplicationId == localLicenseAppId)
               .Select(t => new TestAppointmentByTestTypeDto
               {
                   TestAppointmentId = t.TestAppointmentId,
                   AppointmentDate = t.AppointmentDate,
                   PaidFees = t.PaidFees,
                   IsLocked = t.IsLocked 
               })
                .OrderBy(t => t.TestAppointmentId)
                .ToPagedListAsync(paging, ct);

        public async Task<TestAppointmentDto?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _context.TestAppointments
                .AsNoTracking()
                .Where(t => t.TestAppointmentId == id)
                .ProjectToDto()
                .FirstOrDefaultAsync(ct);

        public async Task<TestAppointmentDto?> GetLastAsync(int localLicenseAppId, int testTypeId, CancellationToken ct = default)
            => await _context.TestAppointments
                .AsNoTracking()
                .Where(t => t.LocalDrivingLicenseApplicationId == localLicenseAppId && t.TestTypeId == testTypeId)
                .OrderByDescending(t => t.AppointmentDate)
                .ThenByDescending(t => t.TestAppointmentId)
                .ProjectToDto()
                .FirstOrDefaultAsync(ct);

        public async Task<TestDto?> GetTestAsync(int testAppointmentId, CancellationToken ct = default)        
           =>  await _context.Tests
                .AsNoTracking()
                .Where(t => t.TestAppointmentId == testAppointmentId)
                .ProjectToDto()
                .FirstOrDefaultAsync(ct);
        

        public async Task<TestAppointmentDto> UpdateAsync(int id, TestAppointmentDto testAppointmentDTO, CancellationToken ct = default)
        {
            var entity = await _context.TestAppointments.FindAsync(id, ct)
                ?? throw new KeyNotFoundException($"test appointment with id {id} not found.");

            entity.AppointmentDate = testAppointmentDTO.AppointmentDate;
            entity.PaidFees = testAppointmentDTO.PaidFees;

            entity.RetakeTestApplicationId = testAppointmentDTO.RetakeTestApplicationId;
            
            await _context.SaveChangesAsync(ct);
            return entity.ToDto();
        }
    }
}
