using DVLD.Contracts.Dtos.Application;
using DVLD.DataAccess.Entities;

namespace DVLD.DataAccess.Mapping
{
    public static class ApplicationMappingExtensions
    {
        public static IQueryable<ApplicationDto> ProjectToDto(this IQueryable<Application> query)
        => query.Select(a => new ApplicationDto
        (
            a.ApplicationId,
            a.ApplicantPersonId,
            a.ApplicationDate,
            a.ApplicationTypeId,
            (byte)a.ApplicationStatus,
            a.LastStatusDate,
            a.PaidFees,
            a.CreatedByUserId
        ));

        public static ApplicationDto ToDto(this Application entity)
            => new ApplicationDto
            {
                ApplicationId = entity.ApplicationId,
                ApplicantPersonId = entity.ApplicantPersonId,
                ApplicationDate = entity.ApplicationDate,
                ApplicationTypeId = entity.ApplicationTypeId,
                ApplicationStatus = (byte)entity.ApplicationStatus,
                LastStatusDate = entity.LastStatusDate,
                PaidFees = entity.PaidFees,
                CreatedByUserId = entity.CreatedByUserId
            };
    }
}
