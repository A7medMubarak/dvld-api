using DVLD.Contracts.Common;
using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.Driver;
using DVLD.Contracts.Requests.Driver;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Common.Validation;

namespace DVLD.Business.Services
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository _drivers;
        private readonly IPersonRepository _people;
        private readonly ICurrentUserService _currentUser;

        public DriverService(IDriverRepository drivers, IPersonRepository people, ICurrentUserService currentUser)
        {
            _drivers = drivers;
            _people = people;
            _currentUser = currentUser;
        }

        //command
        public async Task<DriverDto> CreateAsync(CreateDriverRequest body,CancellationToken cancellationToken)
        {
           Guard.AgainstNull(body, nameof(body));

            await ValidateBody(body);

            body.CreatedDate = DateTime.UtcNow;

            return await _drivers.CreateAsync(ToDto(body), cancellationToken);
        }

        //read
        public async Task<IReadOnlyList<DriverDto>> GetAllAsync(CancellationToken cancellationToken)
            => await _drivers.GetAllAsync(cancellationToken);

        public async Task<PagedResult<DriverDto>> GetPagedAsync(PaginationParams paging, CancellationToken cancellationToken)
            => await _drivers.GetPagedAsync(paging, cancellationToken);

        public async Task<DriverDto?> GetByDriverIdAsync(int driverId, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(driverId, nameof(driverId));

            return await _drivers.GetByDriverIdAsync(driverId,cancellationToken);

        }

        public async Task<DriverDto?> GetByPersonIdAsync(int personId, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(personId, nameof(personId));

            return await _drivers.GetByPersonIdAsync(personId,cancellationToken);
        }

        //Details
        public async Task<DriverDetailedDto?> GetDetailedByDriverIdAsync(int driverId, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(driverId, nameof(driverId));

            var dto = await _drivers.GetByDriverIdAsync(driverId,cancellationToken);
          
            if (dto == null)
                return null;

            return await EnRich(dto);
        }

        public async Task<DriverDetailedDto?> GetDetailedByPersonIdAsync(int personId, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(personId, nameof(personId));

            var dto = await _drivers.GetByPersonIdAsync(personId,cancellationToken);

            if (dto == null)
                return null;

            return await EnRich(dto);
        }

        //Exists
        public async Task<bool> DriverExistsAsync(int driverId, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(driverId, nameof(driverId));

            return await _drivers.ExistsByDriverIdAsync(driverId,cancellationToken);
        }

        public async Task<bool> IsPersonDriverAsync(int personId, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(personId, nameof(personId));
           
            return await _drivers.ExistsByPersonIdAsync(personId,cancellationToken);
        }

        //helpers
        private async Task ValidateBody(CreateDriverRequest body)
        {
            Guard.AgainstNonPositive(body.PersonId, nameof(body.PersonId));

            if (!await _people.ExistsByIdAsync(body.PersonId))
                throw new KeyNotFoundException("Person not exists.");
        }

        private DriverDto ToDto(CreateDriverRequest body, int driverId = 0)
            => new DriverDto
            {
                DriverId = driverId,
                PersonId = body.PersonId,
                CreatedByUserId = _currentUser.UserId,
                CreatedDate = body.CreatedDate
            };

        private async Task<DriverDetailedDto> EnRich(DriverDto dto)
        {
            var person = await  _people.GetByIdAsync(dto.PersonId);

            return new DriverDetailedDto
            {

                DriverId = dto.DriverId,
                PersonId = dto.PersonId,
                CreatedByUserId = dto.CreatedByUserId,
                CreatedDate = dto.CreatedDate,
                Person = person
            };
        }

    }
}
