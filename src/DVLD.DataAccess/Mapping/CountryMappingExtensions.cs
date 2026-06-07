using DVLD.Contracts.Dtos.Country;
using DVLD.DataAccess.Entities;

namespace DVLD.DataAccess.Mapping
{
    public static class CountryMappingExtensions
    {
        public static IQueryable<CountryDto> ProjectToDto(this IQueryable<Country> query)
        => query.Select(c => new CountryDto
        (
            c.CountryId,
            c.CountryName
        ));
    }
}
