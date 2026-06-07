using DVLD.Contracts.Dtos.Test;
using DVLD.DataAccess.Entities;

namespace DVLD.DataAccess.Mapping
{
    public static class TestMappingExtensions
    {
        public static IQueryable<TestDto> ProjectToDto(this IQueryable<Test> query)
            => query.Select(l => new TestDto
            (
                l.TestId,
                l.TestAppointmentId,
                l.TestResult,
                l.Notes,
                l.CreatedByUserId
               
            ));

        public static IQueryable<TestWithApplicantDto> ProjectToTestWithApplicantDto(this IQueryable<Test> query)
            => query.Select(l => new TestWithApplicantDto
            (
                l.TestId,
                l.TestAppointmentId,
                l.TestResult,
                l.Notes,
                l.CreatedByUserId,
                l.TestAppointment.LocalDrivingLicenseApplication.Application.ApplicantPersonId
            ));

        public static Test ToEntity(this TestDto testDTO)
            => new Test
            {
                TestAppointmentId = testDTO.TestAppointmentId,
                TestResult = testDTO.TestResult,
                Notes = testDTO.Notes,
                CreatedByUserId = testDTO.CreatedByUserId
            };

        public static TestDto ToDto(this Test test)
           => new TestDto
           {
               TestId = test.TestId,
               TestAppointmentId = test.TestAppointmentId,
               TestResult = test.TestResult,
               Notes = test.Notes,
               CreatedByUserId = test.CreatedByUserId
           };
    }
}





