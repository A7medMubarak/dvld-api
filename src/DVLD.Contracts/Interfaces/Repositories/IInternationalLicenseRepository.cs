
using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.InternationalLicense;

namespace DVLD.Contracts.Interfaces.Repositories
{
    public interface IInternationalLicenseRepository
    {
        Task <InternationalLicenseDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<InternationalLicenseDto?> GetActiveByDriverIdAsync(int driverId, CancellationToken ct = default);

        Task<IReadOnlyList<InternationalLicenseDto>> GetAllAsync(CancellationToken ct = default);

        Task<PagedResult<InternationalLicenseDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct = default);

        Task<IReadOnlyList<InternationalLicenseDto>> GetAllDriverLicensesAsync(int driverId, CancellationToken ct = default);

        Task<PagedResult<InternationalLicenseDto>> GetPagedDriverLicensesAsync(int driverId, PaginationParams paging, CancellationToken ct = default);

        Task<InternationalLicenseDto> CreateAsync(InternationalLicenseDto internationalLicenseDto, CancellationToken ct = default);
        
        Task<InternationalLicenseDto> UpdateAsync(int id,InternationalLicenseDto internationalLicenseDto, CancellationToken ct = default);
    }
}
