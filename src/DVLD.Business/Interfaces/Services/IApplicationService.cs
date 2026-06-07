using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Application;
using DVLD.Contracts.Requests.Application;

namespace DVLD.Business.Interfaces.Services
{
    public interface IApplicationService
    {
        Task <ApplicationDto?> GetByIdAsync(int applicationId, CancellationToken cancellationToken = default);

        Task <IReadOnlyList<ApplicationDto>> GetAllAsync( CancellationToken cancellationToken = default);

        Task<PagedResult<ApplicationDto>> GetPagedAsync(PaginationParams paging, CancellationToken cancellationToken = default);

        Task<ApplicationDto> CreateAsync(CreateApplicationRequest body, CancellationToken cancellationToken = default);
        
        Task<ApplicationDto> UpdateAsync(int applicationId, UpdateApplicationRequest body, CancellationToken cancellationToken = default);
        
        Task DeleteAsync(int applicationId, CancellationToken cancellationToken = default);
        
        Task<ApplicationDto?> GetActiveAsync(int personId, int typeId, int? licenseClassId, CancellationToken cancellationToken = default);
      
        Task UpdateStatusAsync(int applicationId, short newStatus, CancellationToken cancellationToken = default);

    }
}
