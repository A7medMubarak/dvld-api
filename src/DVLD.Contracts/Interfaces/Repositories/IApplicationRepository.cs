using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Application;
using DVLD.Contracts.Requests.Application;


namespace DVLD.Contracts.Interfaces.Repositories
{
    public interface IApplicationRepository
    {

        // Queries
        Task<ApplicationDto?> GetByIdAsync(int applicationId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ApplicationDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<PagedResult<ApplicationDto>> GetPagedAsync(PaginationParams paging, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int applicationId, CancellationToken cancellationToken = default);
        Task<ApplicationDto?> GetActiveAsync(int personId, int typeId, CancellationToken cancellationToken = default);
        Task<ApplicationDto?> GetActiveForLicenseClassAsync(int personId, int typeId, int licenseClassId, CancellationToken cancellationToken = default);

        // Commands

        Task<ApplicationDto> AddAsync(CreateApplicationRequest dto, DateTime CreatedAt, CancellationToken cancellationToken = default);
        Task<ApplicationDto> UpdateAsync(int applicationId, UpdateApplicationRequest dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int applicationId, CancellationToken cancellationToken = default);
        Task<bool> UpdateStatusAsync(int applicationId, short newStatus, CancellationToken cancellationToken = default);
    }
}
