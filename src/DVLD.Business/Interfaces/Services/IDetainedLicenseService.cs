using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.DetainedLicense;
using DVLD.Contracts.Requests.DetainedLicense;

namespace DVLD.Business.Interfaces.Services
{
    public interface IDetainedLicenseService
    {
        Task<DetainedLicenseDto?> GetByDetainIdAsync(int detainId, CancellationToken cancellationToken = default);

        Task<DetainedLicenseDto?> GetByLicenseIdAsync(int licenseId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<DetainedLicenseViewDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<PagedResult<DetainedLicenseViewDto>> GetPagedAsync(PaginationParams paging, CancellationToken cancellationToken = default);

        Task<DetainedLicenseDto> CreateAsync(CreateDetainedLicenseRequest request, CancellationToken cancellationToken = default);

        Task<bool> ReleaseDetainedLicenseAsync(int detainId, CancellationToken cancellationToken = default);

        Task<bool> IsLicenseDetainedAsync(int licenseId, CancellationToken cancellationToken = default);
    }
}
