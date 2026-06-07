using DVLD.Contracts.Common.Enums;
using DVLD.Contracts.Dtos.LocalLicenseApp;
using DVLD.DataAccess.Entities;

namespace DVLD.DataAccess.Mapping
{
    public static class LocalLicenseAppMappingExtensions
    {
        public static IQueryable<LocalLicenseAppDto> ProjectToDto(this IQueryable<LocalDrivingLicenseApplication> query)
        => query.Select(l => new LocalLicenseAppDto
        (
            l.LocalDrivingLicenseApplicationId,
            l.ApplicationId,
            l.LicenseClassId,
            l.Application.ApplicantPersonId,
            l.Application.ApplicationDate,
            l.Application.ApplicationTypeId,
            (byte)l.Application.ApplicationStatus,
            l.Application.LastStatusDate,
            l.Application.PaidFees,
            l.Application.CreatedByUserId
        ));

        public static IQueryable<LocalLicenseViewDto> ProjectToView(this IQueryable<LocalDrivingLicenseApplication> query)
        => query.Select(l => new LocalLicenseViewDto
       (
           l.LocalDrivingLicenseApplicationId,
           l.LicenseClass.ClassName,
           l.Application.ApplicantPerson.NationalNo,
           string.Concat(
               l.Application.ApplicantPerson.FirstName, " " ,
               l.Application.ApplicantPerson.SecondName, " "  ,
               l.Application.ApplicantPerson.ThirdName ?? "" , " " ,
               l.Application.ApplicantPerson.LastName),
           l.Application.ApplicationDate,
           l.TestAppointments.Count(t => t.Test != null && t.Test.TestResult),
           ((byte)l.Application.ApplicationStatus).ToString()
       ));

        public static Application ToApplicationEntity(this CreateLocalLicenseAppDto localLicenseAppDto)
           => new Application
           {
               ApplicantPersonId = localLicenseAppDto.ApplicantPersonId,
               ApplicationDate = localLicenseAppDto.ApplicationDate,
               ApplicationTypeId = localLicenseAppDto.ApplicationTypeId,
               ApplicationStatus = (enApplicationStatus)localLicenseAppDto.ApplicationStatus,
               LastStatusDate = localLicenseAppDto.LastStatusDate,
               PaidFees = localLicenseAppDto.PaidFees,
               CreatedByUserId = localLicenseAppDto.CreatedByUserId
           };

        public static LocalLicenseAppDto ToDto(this LocalDrivingLicenseApplication entity)
           => new LocalLicenseAppDto
           {
               LocalDrivingLicenseApplicationId = entity.LocalDrivingLicenseApplicationId,
               ApplicationId = entity.ApplicationId,
               LicenseClassId = entity.LicenseClassId,
               ApplicantPersonId = entity.Application.ApplicantPersonId,
               ApplicationDate = entity.Application.ApplicationDate,
               ApplicationTypeId = entity.Application.ApplicationTypeId,
               ApplicationStatus = (byte)entity.Application.ApplicationStatus,
               LastStatusDate = entity.Application.LastStatusDate,
               PaidFees = entity.Application.PaidFees,
               CreatedByUserId = entity.Application.CreatedByUserId
           };
    }

   
}