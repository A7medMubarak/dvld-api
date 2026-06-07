using DVLD.Contracts.Common;
using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.Person;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using DVLD.DataAccess.Mapping;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly AppDbContext _context;

        public PersonRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PersonDto> AddAsync(PersonDto pdto, CancellationToken cancellationToken = default)
        {
            Person entity = new Person
            {
                NationalNo = pdto.NationalNo,
                FirstName = pdto.FirstName,
                SecondName = pdto.SecondName,
                ThirdName = pdto.ThirdName,
                LastName = pdto.LastName,
                DateOfBirth = pdto.DateOfBirth,
                Address = pdto.Address,
                Phone = pdto.Phone,
                Gender = (enGender)pdto.Gender,
                Email = pdto.Email,
                NationalityCountryId = pdto.NationalityCountryId,
                ImagePath = pdto.ImagePath,
                CreatedByUserId = pdto.CreatedByUserId,
            };

            await _context.People.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.ToDto();
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.People.FindAsync(id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"Person with ID {id} not found.");

            _context.People.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default)
            => await _context.People.AnyAsync(p => p.PersonId == id, cancellationToken);

        public async Task<bool> ExistsByNationalNoAsync(string nationalNo, CancellationToken cancellationToken = default)
            => await _context.People.AnyAsync(p => p.NationalNo == nationalNo, cancellationToken);

        public async Task<IReadOnlyList<PersonDto>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _context.People
                .AsNoTracking()
                .ProjectToDto()
                .ToListAsync(cancellationToken);

        public async Task<PagedResult<PersonDto>> GetPagedAsync(PaginationParams paging, CancellationToken cancellationToken = default)
            => await _context.People
                .AsNoTracking()
                .ProjectToDto()
                .ToPagedListAsync(paging, cancellationToken);

        public async Task<PersonDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await _context.People
                .AsNoTracking()
                .ProjectToDto()
                .FirstOrDefaultAsync(p => p.PersonId == id, cancellationToken);

        public async Task<PersonDto?> GetByNationalNoAsync(string nationalNo, CancellationToken cancellationToken = default)
            => await _context.People
                .AsNoTracking()
                .ProjectToDto()
                .FirstOrDefaultAsync(p => p.NationalNo == nationalNo, cancellationToken);

        public async Task<PersonDto> UpdateAsync(int id, PersonDto pdto, CancellationToken cancellationToken = default)
        {
            var entity = await _context.People.FindAsync(id, cancellationToken)
                ?? throw new KeyNotFoundException($"Person with id {id} not found.");

            entity.FirstName = pdto.FirstName;
            entity.SecondName = pdto.SecondName;
            entity.ThirdName = pdto.ThirdName;
            entity.LastName = pdto.LastName;
            entity.DateOfBirth = pdto.DateOfBirth;
            entity.Address = pdto.Address;
            entity.Phone = pdto.Phone;
            entity.Gender = (enGender)pdto.Gender;
            entity.Email = pdto.Email;
            entity.NationalityCountryId = pdto.NationalityCountryId;
            entity.ImagePath = pdto.ImagePath;

            await _context.SaveChangesAsync(cancellationToken);
            
            return entity.ToDto();
        }
    }
}
