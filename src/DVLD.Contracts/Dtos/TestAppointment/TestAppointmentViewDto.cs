namespace DVLD.Contracts.Dtos.TestAppointment
{
    public class TestAppointmentViewDto
    {
        public int TestAppointmentId { get; set; }
        public int LocalDrivingLicenseApplicationId { get; set; }
        public string TestTypeTitle { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public decimal PaidFees { get; set; }
        public string PersonFullName { get; set; } = string.Empty;
        public bool IsLocked { get; set; }

        public TestAppointmentViewDto() { }

        public TestAppointmentViewDto(int testAppointmentId, int localDrivingLicenseApplicationId,
            string testTypeTitle, string className, DateTime appointmentDate,
            decimal paidFees, string personFullName, bool isLocked)
        {
            TestAppointmentId = testAppointmentId;
            LocalDrivingLicenseApplicationId = localDrivingLicenseApplicationId;
            TestTypeTitle = testTypeTitle;
            ClassName = className;
            AppointmentDate = appointmentDate;
            PaidFees = paidFees;
            PersonFullName = personFullName;
            IsLocked = isLocked;
        }
    }
}
