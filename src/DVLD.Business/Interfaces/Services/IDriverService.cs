using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Driver;
using DVLD.Contracts.Requests.Driver;

namespace DVLD.Business.Interfaces.Services
{
    public interface IDriverService
    {
        Task<DriverDto?> GetByDriverIdAsync(int driverId, CancellationToken cancellationToken = default);
        
        Task<DriverDto?> GetByPersonIdAsync(int personId, CancellationToken cancellationToken = default);
        
        Task<DriverDetailedDto?> GetDetailedByDriverIdAsync(int driverId, CancellationToken cancellationToken = default);
        
        Task<DriverDetailedDto?> GetDetailedByPersonIdAsync(int personId, CancellationToken cancellationToken = default);
        
        Task<IReadOnlyList<DriverDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<PagedResult<DriverDto>> GetPagedAsync(PaginationParams paging, CancellationToken cancellationToken = default);
        
        Task<DriverDto> CreateAsync(CreateDriverRequest body,CancellationToken cancellationToken = default);

        Task<bool> DriverExistsAsync(int driverId, CancellationToken cancellationToken = default);

        Task<bool> IsPersonDriverAsync(int personId, CancellationToken cancellationToken = default);
    }
}
