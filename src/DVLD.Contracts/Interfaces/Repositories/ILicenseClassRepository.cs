using DVLD.Contracts.Dtos.LicenseClass;

namespace DVLD.Contracts.Interfaces.Repositories
{
    public interface ILicenseClassRepository
    {
        // Queries
        Task<LicenseClassDto?> GetByIdAsync(int id, CancellationToken cancellationTokenn = default);

        Task<LicenseClassDto?> GetByClassNameAsync(string className, CancellationToken cancellationTokenn = default);

        Task<IReadOnlyList<LicenseClassDto>> GetAllAsync(CancellationToken cancellationTokenn = default);

        // Commands
        Task<LicenseClassDto> CreateAsync(LicenseClassDto licenseClassDto, CancellationToken cancellationTokenn = default);

        Task<LicenseClassDto> UpdateAsync(int id, LicenseClassDto licenseClassDto,CancellationToken cancellationToken= default);

        Task<bool> ExistsAsync(int id, CancellationToken cancellationTokenn=default);
    }
}
