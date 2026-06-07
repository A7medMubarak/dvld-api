using DVLD.Contracts.Dtos.ApplicationType;
using DVLD.Contracts.Requests.ApplicationType;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Common.Validation;

namespace DVLD.Business.Services
{
    public class ApplicationTypeService : IApplicationTypeService
    {
        private readonly IApplicationTypeRepository _repo;

        public ApplicationTypeService(IApplicationTypeRepository repo)
        {
            _repo = repo;
        }

        //Queries
        public async Task<IReadOnlyList<ApplicationTypeDto>> GetAllAsync(CancellationToken cancellationToken) 
            => await _repo.GetAllAsync(cancellationToken);

        public async Task<ApplicationTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(id, nameof(id));

            return await _repo.GetByIdAsync(id, cancellationToken);
        }

        //Commands
        public async Task <ApplicationTypeDto> CreateAsync(CreateApplicationTypeRequest body, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(body, nameof(body));

            ValidateWrite(body);

            var dto = ToDto(body);

            return await _repo.CreateAsync(dto, cancellationToken);
        }

        public async Task <ApplicationTypeDto> UpdateAsync(int id, UpdateApplicationTypeRequest body, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(id, nameof(id));
            Guard.AgainstNull(body, nameof(body));

            if (await _repo.GetByIdAsync(id, cancellationToken) == null)
                throw new KeyNotFoundException($"Application type with id {id} not found.");

            ValidateWrite(body);

            var dto = ToDto(body, id);

            return await _repo.UpdateAsync(id, dto, cancellationToken);

        }

        //Helpers
        private static void ValidateWrite(WriteApplicationTypeRequest body)
        {
            Guard.AgainstNullOrWhiteSpace(body.ApplicationTypeTitle, nameof(body.ApplicationTypeTitle));
            Guard.AgainstInvalidFees(body.ApplicationTypeFees, nameof(body.ApplicationTypeFees));
        }

        private static ApplicationTypeDto ToDto(WriteApplicationTypeRequest body, int id = 0) => new ApplicationTypeDto
            (
                id,
                body.ApplicationTypeTitle,
                body.ApplicationTypeFees
            );
        
    }
}
