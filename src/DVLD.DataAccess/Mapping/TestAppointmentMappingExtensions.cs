using DVLD.Contracts.Dtos.TestAppointment;
using DVLD.DataAccess.Entities;

namespace DVLD.DataAccess.Mapping
{
    public static class TestAppointmentMappingExtensions
    {
        public static IQueryable<TestAppointmentDto> ProjectToDto(this IQueryable<TestAppointment> query)
        => query.Select(t => new TestAppointmentDto
        (
            t.TestAppointmentId,
            t.TestTypeId,
            t.LocalDrivingLicenseApplicationId,
            t.AppointmentDate,
            t.PaidFees,
            t.CreatedByUserId,
            t.IsLocked,
            t.RetakeTestApplicationId
        ));

        public static IQueryable<TestAppointmentViewDto> ProjectToView(this IQueryable<TestAppointment> query)
        => query.Select(t => new TestAppointmentViewDto
        (
            t.TestAppointmentId,
            t.LocalDrivingLicenseApplicationId,
            t.TestType.Title,
            t.LocalDrivingLicenseApplication.LicenseClass.ClassName,
            t.AppointmentDate,
            t.PaidFees,
            t.LocalDrivingLicenseApplication.Application.ApplicantPerson.Fullname,
            t.IsLocked
        ));

        public static TestAppointment ToEntity(this TestAppointmentDto testAppointmentDTO)
            => new TestAppointment
            {
                TestTypeId = testAppointmentDTO.TestTypeId,
                LocalDrivingLicenseApplicationId = testAppointmentDTO.LocalDrivingLicenseApplicationId,
                AppointmentDate = testAppointmentDTO.AppointmentDate,
                PaidFees = testAppointmentDTO.PaidFees,
                CreatedByUserId = testAppointmentDTO.CreatedByUserId,
                IsLocked = testAppointmentDTO.IsLocked,
                RetakeTestApplicationId = testAppointmentDTO.RetakeTestApplicationId
            };

        public static TestAppointmentDto ToDto(this TestAppointment testAppointment)
           => new TestAppointmentDto
           {
               TestAppointmentId = testAppointment.TestAppointmentId,
               TestTypeId = testAppointment.TestTypeId,
               LocalDrivingLicenseApplicationId = testAppointment.LocalDrivingLicenseApplicationId,
               AppointmentDate = testAppointment.AppointmentDate,
               PaidFees = testAppointment.PaidFees,
               CreatedByUserId = testAppointment.CreatedByUserId,
               IsLocked = testAppointment.IsLocked,
               RetakeTestApplicationId = testAppointment.RetakeTestApplicationId,
           };
    }
}





