using DVLD.Contracts.Dtos.ApplicationType;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using DVLD.DataAccess.Mapping;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Repositories
{
    public class ApplicationTypeRepository : IApplicationTypeRepository
    {
        private readonly AppDbContext _context;

        public ApplicationTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationTypeDto> CreateAsync(ApplicationTypeDto applicationTypeDTO, CancellationToken cancellationToken)
        {
            ApplicationType entity = new ApplicationType
            {
                Title = applicationTypeDTO.ApplicationTypeTitle,
                Fees = applicationTypeDTO.ApplicationTypeFees
            };

            await _context.ApplicationTypes.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.ToDto();
        }

        public async Task<IReadOnlyList<ApplicationTypeDto>> GetAllAsync(CancellationToken cancellationToken)
            => await _context.ApplicationTypes
                .AsNoTracking()
                .ProjectToDto()
                .ToListAsync(cancellationToken);

        public async Task<ApplicationTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken )
        {
            var entity = await _context.ApplicationTypes
                .FirstOrDefaultAsync(a => a.ApplicationTypeId == id, cancellationToken);

            return entity == null ? null : entity.ToDto();
        }

        public async Task<ApplicationTypeDto> UpdateAsync(int id, ApplicationTypeDto applicationTypeDTO, CancellationToken cancellationToken)
        {
            var entity = await _context.ApplicationTypes.FindAsync(new object[] { id }, cancellationToken)
                ?? throw new KeyNotFoundException($"Application type with id {id} not found.");

            entity.Title = applicationTypeDTO.ApplicationTypeTitle;
            entity.Fees = applicationTypeDTO.ApplicationTypeFees;

            await _context.SaveChangesAsync(cancellationToken);

            return entity.ToDto();
        }




    }
}
