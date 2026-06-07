using DVLD.Contracts.Dtos.LicenseClass;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Mapping;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Repositories
{
    public partial class LicenseClassRepository : ILicenseClassRepository
    {
        private readonly AppDbContext _context;

        public LicenseClassRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LicenseClassDto> CreateAsync(LicenseClassDto licenseClassDto, CancellationToken cancellationToken = default)
        {
            var entity = licenseClassDto.ToEntity();

            _context.LicenseClasses.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return entity.ToDto();
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
            => await _context.LicenseClasses
                .AsNoTracking()
                .AnyAsync(l => l.LicenseClassId == id, cancellationToken);

        public async Task<IReadOnlyList<LicenseClassDto>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _context.LicenseClasses
                .AsNoTracking()
                .ProjectToDto()  // Extension Method on IQueryable
                .ToListAsync(cancellationToken);

        public async Task<LicenseClassDto?> GetByClassNameAsync(string className, CancellationToken cancellationToken = default)
            => await _context.LicenseClasses
                .AsNoTracking()
                .Where(l => EF.Functions.Like(l.ClassName, className))
                .ProjectToDto() // Extension Method on IQueryable
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<LicenseClassDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await _context.LicenseClasses
                .AsNoTracking()
                .Where(l => l.LicenseClassId == id)
                .ProjectToDto() // Extension Method on IQueryable
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<LicenseClassDto> UpdateAsync(int id, LicenseClassDto licenseClassDto, CancellationToken cancellationToken = default)
        {

            //// ? No SELECT at all � direct UPDATE in one SQL call
            //var affected = await _context.LicenseClasses
            //    .Where(l => l.LicenseClassId == id)
            //    .ExecuteUpdateAsync(setters => setters
            //        .SetProperty(l => l.ClassName, licenseClassDto.ClassName)
            //        .SetProperty(l => l.ClassDescription, licenseClassDto.ClassDescription)
            //        .SetProperty(l => l.MinimumAllowedAge, licenseClassDto.MinimumAllowedAge)
            //        .SetProperty(l => l.DefaultValidityLength, licenseClassDto.DefaultValidityLength)
            //        .SetProperty(l => l.ClassFees, licenseClassDto.ClassFees),
            //    cancellationToken);

            //return affected > 0;

            //// ?? Works but dangerous � marks ALL properties as modified
            //var entity = new LicenseClass { LicenseClassId = id, ClassName = dto.ClassName, ... };
            //_context.LicenseClasses.Attach(entity);
            //_context.Entry(entity).State = EntityState.Modified;
            //await _context.SaveChangesAsync();

            // Fetch - Update
            var entity = await _context.LicenseClasses.FindAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"License class with id {id} not found.");

            entity.ClassName = licenseClassDto.ClassName;
            entity.ClassDescription = licenseClassDto.ClassDescription;
            entity.MinimumAllowedAge = licenseClassDto.MinimumAllowedAge;
            entity.DefaultValidityLength = licenseClassDto.DefaultValidityLength;
            entity.ClassFees = licenseClassDto.ClassFees;

            await _context.SaveChangesAsync(cancellationToken);

            return entity.ToDto();
        }

        // Expression
        //private static readonly Expression<Func<LicenseClass, LicenseClassDto>> EntityToDto =
        //    entity => new LicenseClassDto
        //    (
        //        entity.LicenseClassId,
        //        entity.ClassName,
        //        entity.ClassDescription,
        //        entity.MinimumAllowedAge,
        //        entity.DefaultValidityLength,
        //        entity.ClassFees
        //    );
    }
}
