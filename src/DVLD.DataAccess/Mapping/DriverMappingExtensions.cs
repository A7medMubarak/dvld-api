using DVLD.Contracts.Dtos.Driver;
using DVLD.DataAccess.Entities;

namespace DVLD.DataAccess.Mapping
{
    public static class DriverMappingExtensions
    {
        public static IQueryable<DriverDto> ProjectToDto(this IQueryable<Driver> query)
        => query.Select(d => new DriverDto
        (
            d.DriverId,
            d.PersonId,
            d.CreatedByUserId,
            d.CreatedDate
        ));

        public static DriverDto ToDto(this Driver entity)
            => new DriverDto
            {
                DriverId = entity.DriverId,
                PersonId = entity.PersonId,
                CreatedByUserId = entity.CreatedByUserId,
                CreatedDate = entity.CreatedDate
            };
    }
}
