namespace DVLD.Contracts.Requests.Test
{
    public abstract class TestWriteRequest
    {
        public int TestAppointmentId { get; set; }
        public bool TestResult { get; set; }
        public string? Notes { get; set; }
    }
}
