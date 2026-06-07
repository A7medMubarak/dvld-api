using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.InternationalLicense;
using DVLD.Contracts.Requests.InternationalLicense;

namespace DVLD.Business.Interfaces.Services
{
    public interface IInternationalLicenseService
    {
        Task<InternationalLicenseDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<InternationalLicenseDto?> GetActiveByDriverIdAsync(int driverId, CancellationToken ct = default);

        Task<IReadOnlyList<InternationalLicenseDto>> GetAllAsync(CancellationToken ct = default);

        Task<PagedResult<InternationalLicenseDto>> GetPagedAsync(PaginationParams paging, CancellationToken ct = default);

        Task<IReadOnlyList<InternationalLicenseDto>> GetAllDriverLicensesAsync(int driverId, CancellationToken ct = default);

        Task<PagedResult<InternationalLicenseDto>> GetPagedDriverLicensesAsync(int driverId, PaginationParams paging, CancellationToken ct = default);

        Task<InternationalLicenseDto> CreateAsync(CreateInternationalLicenseRequest request, CancellationToken ct = default);

        Task<InternationalLicenseDto> UpdateAsync(int id, InternationalLicenseDto request, CancellationToken ct = default);

    }
}
