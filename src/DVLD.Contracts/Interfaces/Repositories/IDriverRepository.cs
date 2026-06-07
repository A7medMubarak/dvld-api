using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Driver;

namespace DVLD.Contracts.Interfaces.Repositories
{
    public interface IDriverRepository
    {
      
        Task<DriverDto?> GetByDriverIdAsync(int driverId,CancellationToken cancellationToken = default);
        
        Task<DriverDto?> GetByPersonIdAsync(int personId,CancellationToken cancellationToken = default);
        
        Task<IReadOnlyList<DriverDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<PagedResult<DriverDto>> GetPagedAsync(PaginationParams paging, CancellationToken cancellationToken = default);
        
        Task<DriverDto> CreateAsync (DriverDto driverDto, CancellationToken cancellationToken = default);
        
        Task<bool> ExistsByPersonIdAsync(int personId, CancellationToken cancellationToken = default);
        
        Task<bool> ExistsByDriverIdAsync(int driverId, CancellationToken cancellationToken = default);

    }
}
