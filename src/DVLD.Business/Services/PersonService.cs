using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Person;
using DVLD.Contracts.Requests.Person;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Common.Validation;


namespace DVLD.Business.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _peopleRepo;
        private readonly ICountryRepository _countriesRepo;
        private readonly ICurrentUserService _currentUser;

        public PersonService(IPersonRepository people, ICountryRepository countries, ICurrentUserService currentUser)
        {
            _peopleRepo = people;
            _countriesRepo = countries;
            _currentUser = currentUser;
        }
       
        // Reads
        // --------------------

        public async Task< PersonDto?> GetByIdAsync(int personId,CancellationToken cancellationToken)
        {
            if (personId < 1) 
                throw new ArgumentException("Invalid personId.");

            return await _peopleRepo.GetByIdAsync(personId,cancellationToken);          
        }

        public async Task< PersonDto?> GetByNationalNoAsync(string nationalNo, CancellationToken cancellationToken)
        {
            nationalNo = NormalizeNationalNo(nationalNo);

            return await _peopleRepo.GetByNationalNoAsync(nationalNo, cancellationToken);
        }

        public async Task<IReadOnlyList<PersonDto>> GetAllAsync(CancellationToken cancellationToken) 
            => await _peopleRepo.GetAllAsync(cancellationToken);

        public async Task<PagedResult<PersonDto>> GetPagedAsync(PaginationParams paging, CancellationToken cancellationToken)
            => await _peopleRepo.GetPagedAsync(paging, cancellationToken);

        // Commands
        // --------------------

        public async Task<PersonDto> CreateAsync(CreatePersonRequest body, CancellationToken cancellationToken)
        {
            if(body == null) 
                throw new ArgumentException("body is required.");
            
            ValidateWrite(body);

            // Uniqueness example (adjust if your DB already enforces it)
            if (await _peopleRepo.ExistsByNationalNoAsync(body.NationalNo,cancellationToken))
            {
                throw new ArgumentException("NationalNo already exists.");
            }

            var dto = ToDto(body);

            return await _peopleRepo.AddAsync(dto, cancellationToken);
        }

        public async Task DeleteAsync(int personId, CancellationToken cancellationToken)
        {
            if (personId < 1)
                throw new ArgumentException("Invalid personId.");

            if (!await _peopleRepo.ExistsByIdAsync(personId, cancellationToken))
                throw new KeyNotFoundException("Person not found.");

            await _peopleRepo.DeleteAsync(personId, cancellationToken);
        }

        public async Task<PersonDto> UpdateAsync(int personId, UpdatePersonRequest body, CancellationToken cancellationToken)
        {
            if (personId < 1)
                throw new ArgumentException("Invalid personId.");
           
            if (body == null)
                throw new ArgumentException("Body is required.");

            ValidateWrite(body);

            if (!await _peopleRepo.ExistsByIdAsync(personId,cancellationToken))
                throw new KeyNotFoundException("Person not found.");

            // If NationalNo can be changed, ensure uniqueness
            var newNationalNo = body.NationalNo;
            var existing = await _peopleRepo.GetByIdAsync(personId,cancellationToken);

            if (existing == null)
                throw new KeyNotFoundException("Person not found.");

            if(!string.Equals(existing.NationalNo, newNationalNo, StringComparison.OrdinalIgnoreCase)
                && await _peopleRepo.ExistsByNationalNoAsync(newNationalNo,cancellationToken))
            {
                throw new ArgumentException("nationalNo already exists.");
            }

            var dto = ToDto(body, personId);

            return await _peopleRepo.UpdateAsync(personId, dto, cancellationToken);
        }

        // Exists 
        // --------------------

        public async Task<bool> ExistsByIdAsync(int personId, CancellationToken cancellationToken)
        {
            if (personId < 1) 
                throw new ArgumentException("Invalid personId.");

            return await _peopleRepo.ExistsByIdAsync(personId,cancellationToken);
        }

        public async Task<bool> ExistsByNationalNoAsync(string nationalNo, CancellationToken cancellationToken)
        {
            nationalNo = NormalizeNationalNo(nationalNo);

            return await _peopleRepo.ExistsByNationalNoAsync(nationalNo,cancellationToken);
        }
        
        // Details 
        // --------------------

        public async Task <PersonDetailedDto?> GetDetailsByIdAsync(int personId, CancellationToken cancellationToken)
        {
            if (personId < 1) 
                throw new ArgumentException("Invalid personId.");

            var p = await _peopleRepo.GetByIdAsync(personId,cancellationToken);
           
            if (p == null) 
                return null;

            return await Enrich(p); 
        }

        public async Task<PersonDetailedDto?> GetDetailsByNationalNoAsync(string nationalNo, CancellationToken cancellationToken)
        {
            nationalNo = NormalizeNationalNo(nationalNo);

            var p = await _peopleRepo.GetByNationalNoAsync(nationalNo,cancellationToken);

            if (p == null) 
                return null;

            return await Enrich(p);
        }

        // Helpers
        // --------------------

        private static void ValidateWrite(PersonWriteRequest body)
        {
            Guard.AgainstNullOrWhiteSpace(body.FirstName, nameof(body.FirstName));
            Guard.AgainstNullOrWhiteSpace(body.SecondName, nameof(body.SecondName));
            Guard.AgainstNullOrWhiteSpace(body.LastName, nameof(body.LastName));
            Guard.AgainstNullOrWhiteSpace(body.NationalNo, nameof(body.NationalNo));
            Guard.AgainstNonPositive(body.NationalityCountryId, nameof(body.NationalityCountryId));
        }

        private static string NormalizeNationalNo(string nationalNo)
        {
            Guard.AgainstNullOrWhiteSpace(nationalNo, nameof(nationalNo));

            return nationalNo.Trim();
        }

        private async Task< PersonDetailedDto> Enrich (PersonDto p)
        {
            var country = await _countriesRepo.GetByIdAsync(p.NationalityCountryId);

            return new PersonDetailedDto
            {
                PersonId = p.PersonId,
                FirstName = p.FirstName,
                SecondName = p.SecondName,
                ThirdName = p.ThirdName,
                LastName = p.LastName,
                DateOfBirth = p.DateOfBirth,
                Address = p.Address,
                Phone = p.Phone,
                Gender = p.Gender,
                Email = p.Email,
                NationalityCountryId = p.NationalityCountryId,
                NationalNo = p.NationalNo,
                ImagePath = p.ImagePath,

                //Country = country,
                CountryName = country?.CountryName
            };
        }

        private PersonDto ToDto(PersonWriteRequest r, int personId = 0) => new PersonDto
        {
            PersonId = personId,
            FirstName = r.FirstName,
            SecondName = r.SecondName,
            ThirdName = r.ThirdName,
            LastName = r.LastName,
            DateOfBirth = r.DateOfBirth,
            Address = r.Address,
            Phone = r.Phone,
            Gender = r.Gender,
            Email = r.Email,
            NationalityCountryId = r.NationalityCountryId,
            NationalNo = r.NationalNo,
            ImagePath = r.ImagePath,
            CreatedByUserId = _currentUser.UserId
        };

    }
}
