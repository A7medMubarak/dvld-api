namespace DVLD.DataAccess.Entities
{
    public class Test
    {
        public int TestId { get; set; }
        public int TestAppointmentId { get; set; }
        public bool TestResult { get; set; }
        public string? Notes { get; set; }
        public int CreatedByUserId { get; set; }

        public TestAppointment TestAppointment { get; set; } = null!;
        public User CreatedByUser { get; set; } = null!;
    }
}
