using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Driver;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using DVLD.DataAccess.Mapping;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Repositories
{
    public class DriverRepository : IDriverRepository
    {
        private readonly AppDbContext _context;

        public DriverRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DriverDto> CreateAsync(DriverDto driverDto, CancellationToken cancellationToken = default)
        {
            var entity = new Driver
            {
                PersonId = driverDto.PersonId,
                CreatedByUserId = driverDto.CreatedByUserId,
                CreatedDate = driverDto.CreatedDate
            };

            _context.Drivers.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.ToDto();
        }

        public async Task<bool> ExistsByDriverIdAsync(int driverId, CancellationToken cancellationToken = default)
            => await _context.Drivers
                .AsNoTracking()
                .AnyAsync(d => d.DriverId == driverId, cancellationToken);

        public async Task<bool> ExistsByPersonIdAsync(int personId, CancellationToken cancellationToken = default)
            => await _context.Drivers
                .AsNoTracking()
                .AnyAsync(d => d.PersonId == personId, cancellationToken);

        public async Task<IReadOnlyList<DriverDto>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _context.Drivers
                .AsNoTracking()
                .ProjectToDto()
                .ToListAsync(cancellationToken);

        public async Task<PagedResult<DriverDto>> GetPagedAsync(PaginationParams paging, CancellationToken cancellationToken = default)
            => await _context.Drivers
                .AsNoTracking()
                .ProjectToDto()
                .OrderBy(d => d.DriverId)
                .ToPagedListAsync(paging, cancellationToken);

        public async Task<DriverDto?> GetByDriverIdAsync(int driverId, CancellationToken cancellationToken = default)
            => await _context.Drivers
                .AsNoTracking()
                .Where(d => d.DriverId == driverId)
                .ProjectToDto()
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<DriverDto?> GetByPersonIdAsync(int personId, CancellationToken cancellationToken = default)
            => await _context.Drivers
                .AsNoTracking()
                .Where(d => d.PersonId == personId)
                .ProjectToDto()
                .FirstOrDefaultAsync( cancellationToken);
    }
}
