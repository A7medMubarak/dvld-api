namespace DVLD.Contracts.Dtos.Test
{
    public class TestDto
    {
        public int TestId { get; set; }
        public int TestAppointmentId { get; set; }
        public bool TestResult { get; set; }
        public string? Notes { get; set; }
        public int CreatedByUserId { get; set; }

        public TestDto() { }

        public TestDto(int testId, int testAppointmentId, bool testResult, string? notes, int createdByUserId)
        {
            TestId = testId;
            TestAppointmentId = testAppointmentId;
            TestResult = testResult;
            Notes = notes;
            CreatedByUserId = createdByUserId;
        }
    }
}
