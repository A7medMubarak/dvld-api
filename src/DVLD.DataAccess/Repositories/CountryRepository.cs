using DVLD.Contracts.Dtos.Country;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Mapping;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly AppDbContext _context;

        public CountryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<CountryDto>> GetAllAsync(CancellationToken cancellationToken)
            => await _context.Countries
                .AsNoTracking()
                .ProjectToDto()
                .ToListAsync(cancellationToken);
        
        public async Task<CountryDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
            => await _context.Countries
                .AsNoTracking()
                .Where(c => c.CountryId == id)
                .ProjectToDto()
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<CountryDto?> GetByNameAsync(string name, CancellationToken cancellationToken)
            => await _context.Countries
                .AsNoTracking()
                .Where(c => c.CountryName == name)
                .ProjectToDto()
                .FirstOrDefaultAsync(cancellationToken);
    }
}
