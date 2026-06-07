using DVLD.Contracts.Dtos.ApplicationType;
using DVLD.DataAccess.Entities;

namespace DVLD.DataAccess.Mapping
{
    public static class ApplicationTypeMappingExtensions
    {
        public static IQueryable<ApplicationTypeDto> ProjectToDto(this IQueryable<ApplicationType> query)
        => query.Select(a => new ApplicationTypeDto
        (
            a.ApplicationTypeId,
            a.Title,
            a.Fees
        ));

        public static ApplicationTypeDto ToDto(this ApplicationType entity)
            => new ApplicationTypeDto
            {
                ApplicationTypeId = entity.ApplicationTypeId,
                ApplicationTypeTitle = entity.Title,
                ApplicationTypeFees = entity.Fees
            };

        public static ApplicationType ToEntity(this ApplicationTypeDto dto)
            => new ApplicationType
            {
                Title = dto.ApplicationTypeTitle,
                Fees = dto.ApplicationTypeFees
            };
    }
}
