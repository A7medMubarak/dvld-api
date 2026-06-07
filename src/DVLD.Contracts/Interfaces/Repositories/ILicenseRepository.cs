using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.License;

namespace DVLD.Contracts.Interfaces.Repositories
{
    public interface ILicenseRepository
    {       
        Task<LicenseDto?> GetByIdAsync(int licenseId, CancellationToken ct = default);

        Task<IReadOnlyList<LicenseDto>> GetAllAsync(CancellationToken ct = default);

        Task<PagedResult<LicenseDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct = default);

        Task<IReadOnlyList<DriverLicensesDto>> GetDriverLicensesAsync(int driverId, CancellationToken ct = default);

        Task<PagedResult<DriverLicensesDto>> GetPagedDriverLicensesAsync(int driverId, PaginationParams paging, CancellationToken ct = default);

        Task<LicenseDto?> GetActiveLicenseForPersonAsync(int personId, int licenseClassId, CancellationToken ct = default);

        Task<LicenseDto> CreateAsync(LicenseDto licenseDTO, CancellationToken ct = default);
        
        Task<LicenseDto> UpdateAsync(int licenseId, LicenseDto licenseDTO, CancellationToken ct = default);
      
        Task<bool >DeactivateAsync(int licenseId, CancellationToken ct = default);

        Task<bool >ExistsAsync(int licenseId, CancellationToken ct = default);
    }
}
