using DVLD.Contracts.Dtos.DetainedLicense;
using DVLD.DataAccess.Entities;

namespace DVLD.DataAccess.Mapping
{
    public static class DetainedLicenseMappingExtensions
    {
        public static IQueryable<DetainedLicenseDto> ProjectToDto(this IQueryable<DetainedLicense> query)
        => query.Select(d => new DetainedLicenseDto
        (
            d.DetainId,
            d.LicenseId,
            d.DetainDate,
            d.FineFees,
            d.CreatedByUserId,
            d.IsReleased,
            d.ReleaseDate,
            d.ReleaseByUserId,
            d.ReleaseApplicationId
        ));

        public static IQueryable<DetainedLicenseViewDto> ProjectToView(this IQueryable<DetainedLicense> query)
        => query.Select(d => new DetainedLicenseViewDto
        (
            d.DetainId,
            d.LicenseId,
            d.DetainDate,
            d.IsReleased,
            d.FineFees,
            d.ReleaseDate,
            d.License.Driver.Person.NationalNo,
            d.License.Driver.Person.FirstName + " " +
            d.License.Driver.Person.SecondName + " " +
            (d.License.Driver.Person.ThirdName ?? "") + " " +
            d.License.Driver.Person.LastName,
            d.ReleaseApplicationId
        ));

        public static DetainedLicenseDto ToDto(this DetainedLicense entity)
            => new DetainedLicenseDto
            {
                DetainId = entity.DetainId,
                LicenseId = entity.LicenseId,
                DetainDate = entity.DetainDate,
                FineFees = entity.FineFees,
                CreatedByUserId = entity.CreatedByUserId,
                IsReleased = entity.IsReleased,
                ReleaseDate = entity.ReleaseDate,
                ReleaseByUserId = entity.ReleaseByUserId,
                ReleaseApplicationId = entity.ReleaseApplicationId
            };

        public static DetainedLicense ToEntity(this DetainedLicenseDto dto)
            => new DetainedLicense
            {
                LicenseId = dto.LicenseId,
                DetainDate = dto.DetainDate,
                FineFees = dto.FineFees,
                CreatedByUserId = dto.CreatedByUserId,
                IsReleased = false,
                ReleaseDate = null,
                ReleaseByUserId = null,
                ReleaseApplicationId = null
            };
    }
}
