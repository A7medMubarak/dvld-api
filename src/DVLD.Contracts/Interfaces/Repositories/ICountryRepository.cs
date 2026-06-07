using DVLD.Contracts.Dtos.Country;

namespace DVLD.Contracts.Interfaces.Repositories
{
    public interface ICountryRepository
    {
        Task<CountryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<CountryDto?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<CountryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
