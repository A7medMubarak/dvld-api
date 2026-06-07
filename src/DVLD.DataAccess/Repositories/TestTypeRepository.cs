using DVLD.Contracts.Dtos.TestType;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Mapping;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Repositories
{
    public class TestTypeRepository : ITestTypeRepository
    {
        private readonly AppDbContext _context;

        public TestTypeRepository(AppDbContext context)
        {
            _context = context;
        }
       
        public async Task<TestTypeDto> CreateAsync(TestTypeDto testTypeDto, CancellationToken ct = default)
        {
            var entity = testTypeDto.ToEntity();

            await _context.TestTypes.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);

            return entity.ToDto();
        }

        public async Task<IReadOnlyList<TestTypeDto>> GetAllAsync(CancellationToken ct = default)
            => await _context.TestTypes
                .AsNoTracking()
                .ProjectToDto()
                .ToListAsync(ct);

        public async Task<TestTypeDto?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _context.TestTypes
                .AsNoTracking()
                .Where(t => t.TestTypeId == id)
                .ProjectToDto()
                .FirstOrDefaultAsync(ct);

        public async Task<TestTypeDto> UpdateAsync(int id, TestTypeDto testTypeDto, CancellationToken ct = default)
        {
            var entity = await _context.TestTypes.FirstOrDefaultAsync(t => t.TestTypeId == id, ct)
                ?? throw new KeyNotFoundException($"Test type with id {id} not found.");

            entity.Description = testTypeDto.Description;
            entity.Title = testTypeDto.Title;
            entity.Fees = testTypeDto.Fees;

            await _context.SaveChangesAsync(ct);
            return entity.ToDto();
        }
    }
}
