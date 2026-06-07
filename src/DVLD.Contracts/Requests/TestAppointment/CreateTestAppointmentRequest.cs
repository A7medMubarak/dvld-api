namespace DVLD.Contracts.Requests.TestAppointment
{
    public class CreateTestAppointmentRequest
    {
        public int LocalDrivingLicenseApplicationId { get; set; }
        public int TestTypeId { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
