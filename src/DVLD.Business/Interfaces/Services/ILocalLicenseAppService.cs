using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.LocalLicenseApp;
using DVLD.Contracts.Requests.LocalLicenseApp;

namespace DVLD.Business.Interfaces.Services
{
    public interface ILocalLicenseAppService
    {
        Task<LocalLicenseAppDto?> GetByLocalLicenseAppIdAsync(int localLicenseAppId, CancellationToken ct = default);

        Task<LocalLicenseAppDto?> GetByApplicationIdAsync(int applicationId, CancellationToken ct = default);

        Task<IReadOnlyList<LocalLicenseViewDto>> GetAllAsync(CancellationToken ct = default);

        Task<PagedResult<LocalLicenseViewDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct = default);

        Task<LocalLicenseAppDto> CreateAsync(CreateLocalLicenseAppRequest body, CancellationToken ct = default);

        Task<LocalLicenseAppDto> UpdateAsync(int id, UpdateLocalLicenseAppRequest body, CancellationToken ct = default);

        Task DeleteAsync(int id, CancellationToken ct = default);

        Task<bool> ExistsAsync(int id, CancellationToken ct = default);

        Task<bool> HasPassedTestAsync(int localLicenseAppId, int testTypeId, CancellationToken ct = default);

        Task<bool> HasAttendedTestAsync(int localLicenseAppId, int testTypeId, CancellationToken ct = default);

        Task<byte> GetTestAttemptCountAsync(int localLicenseAppId, int testTypeId, CancellationToken ct = default);

        Task<bool> HasActiveTestAppointmentAsync(int localLicenseAppId, int testTypeId, CancellationToken ct = default);

    }
}
