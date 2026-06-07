using DVLD.Contracts.Dtos.ApplicationType;

namespace DVLD.Contracts.Interfaces.Repositories
{
    public interface IApplicationTypeRepository
    {
        Task<ApplicationTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ApplicationTypeDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<ApplicationTypeDto> CreateAsync(ApplicationTypeDto applicationTypeDTO, CancellationToken cancellationToken = default);

        Task<ApplicationTypeDto> UpdateAsync(int id, ApplicationTypeDto applicationTypeDTO, CancellationToken cancellationToken = default);
    }
}
