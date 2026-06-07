using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.DetainedLicense;

namespace DVLD.Contracts.Interfaces.Repositories
{
    public interface IDetainedLicenseRepository
    {
        Task<DetainedLicenseDto?> GetByDetainIdAsync(int detainId, CancellationToken cancellationToken = default);

        Task<DetainedLicenseDto?> GetByLicenseIdAsync(int licenseId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<DetainedLicenseViewDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<PagedResult<DetainedLicenseViewDto>> GetPagedAsync(PaginationParams paging, CancellationToken cancellationToken = default);

        Task<DetainedLicenseDto> CreateAsync(DetainedLicenseDto dto, CancellationToken cancellationToken = default);

        Task<bool> ReleaseDetainedLicenseAsync(int detainId, int releasedByUserId, int releaseApplicationId, CancellationToken cancellationToken = default);

        Task<bool> IsLicenseDetainedAsync(int licenseId, CancellationToken cancellationToken = default);


    }
}
