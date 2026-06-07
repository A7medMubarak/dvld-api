using DVLD.Contracts.Common;
using DVLD.Contracts.Dtos.Person;


namespace DVLD.Contracts.Interfaces.Repositories
{
    public interface IPersonRepository
    {
        Task<PersonDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<PersonDto?> GetByNationalNoAsync(string nationalNo, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PersonDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<PagedResult<PersonDto>> GetPagedAsync(PaginationParams paging, CancellationToken cancellationToken = default);

        Task<PersonDto> AddAsync(PersonDto pdto, CancellationToken cancellationToken = default);

        Task<PersonDto> UpdateAsync(int id, PersonDto pdto, CancellationToken cancellationToken = default);

        Task DeleteAsync(int id, CancellationToken cancellationToken = default);

        Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<bool> ExistsByNationalNoAsync(string nationalNo, CancellationToken cancellationToken = default);
    }
}
