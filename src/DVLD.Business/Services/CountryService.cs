using DVLD.Contracts.Dtos.Country;
using DVLD.Contracts.Interfaces.Repositories;
using DVLD.Business.Interfaces.Services;
using DVLD.Business.Common.Validation;

namespace DVLD.Business.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _repo;

        public CountryService(ICountryRepository repo)
        {
            _repo = repo;
        }

        public async Task<IReadOnlyList<CountryDto>> GetAllAsync(CancellationToken cancellationToken)
            => await _repo.GetAllAsync(cancellationToken);

        public async Task<CountryDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            Guard.AgainstNonPositive(id, nameof(id));
            return await _repo.GetByIdAsync(id, cancellationToken);
        }

        public async Task<CountryDto?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            Guard.AgainstNullOrWhiteSpace(name, nameof(name));
            return await _repo.GetByNameAsync(name, cancellationToken);
        }
    }
}
