using DVLD.Contracts.Dtos.License;
using DVLD.DataAccess.Entities;

namespace DVLD.DataAccess.Mapping
{
    public static class LicenseMappingExtensions
    {
        public static IQueryable<LicenseDto> ProjectToDto(this IQueryable<License> query)
        => query.Select(l => new LicenseDto
        (
            l.LicenseId,
            l.ApplicationId,
            l.DriverId,
            l.LicenseClassId,
            l.IssueDate,
            l.ExpirationDate,
            l.Notes,
            l.PaidFees,
            l.IsActive,
            l.IssueReason,
            l.CreatedByUserId
        ));

        public static IQueryable<DriverLicensesDto> ProjectToDriverLicensesDto(this IQueryable<License> query)
       => query.Select(l => new DriverLicensesDto
       (
           l.LicenseId,
           l.ApplicationId,
           l.LicenseClass.ClassName,
           l.IssueDate,
           l.ExpirationDate,
           l.IsActive
       ));

        public static LicenseDto ToDto(this License license)
             => new LicenseDto
             {
                 LicenseId = license.LicenseId,
                 LicenseClassId = license.LicenseClassId,
                 ApplicationId = license.ApplicationId,
                 DriverId = license.DriverId,
                 IssueDate = license.IssueDate,
                 ExpirationDate = license.ExpirationDate,
                 IssueReason = license.IssueReason,
                 Notes = license.Notes,
                 PaidFees = license.PaidFees,
                 IsActive = license.IsActive,
                 CreatedByUserId = license.CreatedByUserId
             };

        public static License ToEntity(this LicenseDto licenseDto)
           => new License
           {
               LicenseClassId = licenseDto.LicenseClassId,
               ApplicationId = licenseDto.ApplicationId,
               DriverId = licenseDto.DriverId,
               IssueDate = licenseDto.IssueDate,
               ExpirationDate = licenseDto.ExpirationDate,
               IssueReason = licenseDto.IssueReason,
               Notes = licenseDto.Notes,
               PaidFees = licenseDto.PaidFees,
               IsActive = licenseDto.IsActive,
               CreatedByUserId = licenseDto.CreatedByUserId
           };


    }
}