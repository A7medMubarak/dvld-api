using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.License;
using DVLD.Contracts.Requests.License;

namespace DVLD.Business.Interfaces.Services
{
    public interface ILicenseService
    {
        Task<LicenseDto?> GetByIdAsync(int licenseId, CancellationToken ct = default);

        Task<IReadOnlyList<LicenseDto>> GetAllAsync(CancellationToken ct = default);

        Task<PagedResult<LicenseDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct = default);

        Task<IReadOnlyList<DriverLicensesDto>> GetDriverLicensesAsync(int driverId, CancellationToken ct = default);

        Task<PagedResult<DriverLicensesDto>> GetPagedDriverLicensesAsync(int driverId, PaginationParams paging, CancellationToken ct = default);

        Task<LicenseDto?> GetActiveLicenseForPersonAsync(int personId, int licenseClassId, CancellationToken ct = default);

        Task<LicenseDto>CreateAsync(CreateLicenseRequest body, CancellationToken ct = default);

        Task<LicenseDto> UpdateAsync(int licenseId, UpdateLicenseRequest body, CancellationToken ct = default);

        Task<bool> DeactivateAsync(int licenseId, CancellationToken ct = default);

        Task<bool> ExistsAsync(int licenseId, CancellationToken ct = default);
    }
}
