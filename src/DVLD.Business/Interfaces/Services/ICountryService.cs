using DVLD.Contracts.Dtos.Country;

namespace DVLD.Business.Interfaces.Services
{
    public interface ICountryService
    {
        Task <CountryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task <CountryDto?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        Task <IReadOnlyList<CountryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
