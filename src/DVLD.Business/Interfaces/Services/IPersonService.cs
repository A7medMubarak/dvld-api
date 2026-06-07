using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Person;
using DVLD.Contracts.Requests.Person;

namespace DVLD.Business.Interfaces.Services
{
    public interface IPersonService
    {
        Task<PersonDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PersonDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<PagedResult<PersonDto>> GetPagedAsync(PaginationParams paging, CancellationToken cancellationToken = default);

        Task<PersonDto?> GetByNationalNoAsync(string nationalNo ,CancellationToken cancellationToken = default);

        Task<PersonDetailedDto?> GetDetailsByIdAsync(int personId, CancellationToken cancellationToken = default);

        Task<PersonDetailedDto?> GetDetailsByNationalNoAsync(string nationalNo, CancellationToken cancellationToken = default);

        Task<PersonDto> CreateAsync(CreatePersonRequest body, CancellationToken cancellationToken = default);

        Task<PersonDto> UpdateAsync(int personId, UpdatePersonRequest body, CancellationToken cancellationToken = default);

        Task DeleteAsync(int id, CancellationToken cancellationToken = default);

        Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<bool> ExistsByNationalNoAsync(string nationalNo, CancellationToken cancellationToken = default);
    }
}
