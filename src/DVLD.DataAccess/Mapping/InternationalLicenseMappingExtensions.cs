using DVLD.Contracts.Dtos.InternationalLicense;
using DVLD.DataAccess.Entities;

namespace DVLD.DataAccess.Mapping
{
    public static class InternationalLicenseMappingExtensions
    {
        public static IQueryable<InternationalLicenseDto> ProjectToDto(this IQueryable<InternationalLicense> query)
        => query.Select(l => new InternationalLicenseDto
        (
            l.InternationalLicenseId,
            l.ApplicationId,
            l.DriverId,
            l.IssuedUsingLocalLicenseId,
            l.IssueDate,
            l.ExpirationDate,
            l.IsActive,
            l.CreatedByUserId
        ));

        public static InternationalLicense ToEntity(this InternationalLicenseDto internationalLicenseDto)
            => new InternationalLicense
            {
                ApplicationId = internationalLicenseDto.ApplicationId,
                DriverId  =internationalLicenseDto.DriverId,
                IssuedUsingLocalLicenseId = internationalLicenseDto.IssuedUsingLocalLicenseId,
                IssueDate = internationalLicenseDto.IssueDate,
                ExpirationDate = internationalLicenseDto.ExpirationDate,
                IsActive = internationalLicenseDto.IsActive,
                CreatedByUserId = internationalLicenseDto.CreatedByUserId
            };

        public static InternationalLicenseDto ToDto(this InternationalLicense internationalLicense)
           => new InternationalLicenseDto
           {
               ApplicationId = internationalLicense.ApplicationId,
               DriverId = internationalLicense.DriverId,
               IssuedUsingLocalLicenseId = internationalLicense.IssuedUsingLocalLicenseId,
               IssueDate = internationalLicense.IssueDate,
               ExpirationDate = internationalLicense.ExpirationDate,
               IsActive = internationalLicense.IsActive,
               CreatedByUserId = internationalLicense.CreatedByUserId
           };
    }
}
