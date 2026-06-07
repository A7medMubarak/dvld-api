namespace DVLD.Contracts.Dtos.TestAppointment
{
    public class TestAppointmentDto
    {
        public int TestAppointmentId { get; set; }
        public int TestTypeId { get; set; }
        public int LocalDrivingLicenseApplicationId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserId { get; set; }
        public bool IsLocked { get; set; }
        public int? RetakeTestApplicationId { get; set; }

        public TestAppointmentDto() { }

        public TestAppointmentDto(int testAppointmentId, int testTypeId, int localDrivingLicenseApplicationId,
            DateTime appointmentDate, decimal paidFees, int createdByUserId,
            bool isLocked, int? retakeTestApplicationId)
        {
            TestAppointmentId = testAppointmentId;
            TestTypeId = testTypeId;
            LocalDrivingLicenseApplicationId = localDrivingLicenseApplicationId;
            AppointmentDate = appointmentDate;
            PaidFees = paidFees;
            CreatedByUserId = createdByUserId;
            IsLocked = isLocked;
            RetakeTestApplicationId = retakeTestApplicationId;
        }
    }
}
