using DVLD.Contracts.Dtos.ApplicationType;
using DVLD.Contracts.Requests.ApplicationType;

namespace DVLD.Business.Interfaces.Services
{
    public interface IApplicationTypeService
    {
        Task<ApplicationTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ApplicationTypeDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<ApplicationTypeDto> CreateAsync(CreateApplicationTypeRequest body, CancellationToken cancellationToken = default);
        
        Task<ApplicationTypeDto> UpdateAsync(int id, UpdateApplicationTypeRequest body, CancellationToken cancellationToken = default);

    }
}
