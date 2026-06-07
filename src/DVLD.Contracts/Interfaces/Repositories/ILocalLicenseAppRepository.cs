using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.LocalLicenseApp;

namespace DVLD.Contracts.Interfaces.Repositories
{
    public interface ILocalLicenseAppRepository
    {
        Task<LocalLicenseAppDto?> GetByLocalLicenseAppIdAsync(int localLicenseAppId,CancellationToken ct = default);
   
        Task<LocalLicenseAppDto?> GetByApplicationIdAsync(int applicationId, CancellationToken ct = default);

        Task<IReadOnlyList <LocalLicenseViewDto>> GetAllAsync( CancellationToken ct = default);

        Task<PagedResult<LocalLicenseViewDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct = default);

        Task<LocalLicenseAppDto> CreateAsync(CreateLocalLicenseAppDto localLicenseAppDTO, CancellationToken ct = default);

        Task<LocalLicenseAppDto> UpdateAsync(int id, LocalLicenseAppDto localLicenseAppDTO, CancellationToken ct = default);

        Task DeleteAsync(int id, CancellationToken ct = default);

        Task<bool> ExistsAsync(int id, CancellationToken ct = default);

        Task<bool> HasPassedTestAsync(int localLicenseAppId, int testTypeId, CancellationToken ct = default);

        Task<bool> HasAttendedTestAsync(int localLicenseAppId, int testTypeId, CancellationToken ct = default);

        Task<byte> GetTestAttemptCountAsync(int localLicenseAppId, int testTypeId, CancellationToken ct = default);
       
        Task<bool> HasActiveTestAppointmentAsync(int localLicenseAppId, int testTypeId, CancellationToken ct=default);
    }
}
