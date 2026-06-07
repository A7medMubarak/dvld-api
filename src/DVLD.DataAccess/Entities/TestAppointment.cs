namespace DVLD.DataAccess.Entities
{
    public class TestAppointment
    {
        public int TestAppointmentId { get; set; }
        public int TestTypeId { get; set; }
        public int LocalDrivingLicenseApplicationId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserId { get; set; }
        public bool IsLocked { get; set; }
        public int? RetakeTestApplicationId { get; set; }

        public TestType TestType { get; set; } = null!;
        public Test? Test { get; set; }
        public LocalDrivingLicenseApplication LocalDrivingLicenseApplication { get; set; } = null!;
        public User CreatedByUser { get; set; } = null!;
        public Application? RetakeTestApplication { get; set; }
    }
}
