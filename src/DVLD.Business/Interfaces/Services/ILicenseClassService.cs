using DVLD.Contracts.Dtos.LicenseClass;
using DVLD.Contracts.Requests.LicenseClass;

namespace DVLD.Business.Interfaces.Services
{
    public interface ILicenseClassService
    {
        // Queries
        Task<LicenseClassDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default); 

        Task<LicenseClassDto?> GetByClassNameAsync(string className,CancellationToken cancellationToken = default); 

        Task<IReadOnlyList<LicenseClassDto>> GetAllAsync(CancellationToken cancellationToken = default);

        // Commands
        Task<LicenseClassDto> CreateAsync(CreateLicenseClassRequest body, CancellationToken cancellationToken = default);

        Task<LicenseClassDto> UpdateAsync(int id, UpdateLicenseClassRequest body, CancellationToken cancellationToken = default);
    }
}
