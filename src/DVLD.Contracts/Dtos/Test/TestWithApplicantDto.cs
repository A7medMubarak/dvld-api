namespace DVLD.Contracts.Dtos.Test
{
    public class TestWithApplicantDto
    {
        public int TestId { get; set; }
        public int TestAppointmentId { get; set; }
        public bool TestResult { get; set; }
        public string? Notes { get; set; }
        public int CreatedByUserId { get; set; }
        public int ApplicantPersonId { get; set; }

        public TestWithApplicantDto() { }

        public TestWithApplicantDto(int testId, int testAppointmentId, bool testResult,
            string? notes, int createdByUserId, int applicantPersonId)
        {
            TestId = testId;
            TestAppointmentId = testAppointmentId;
            TestResult = testResult;
            Notes = notes;
            CreatedByUserId = createdByUserId;
            ApplicantPersonId = applicantPersonId;
        }
    }
}
