namespace DVLD.Contracts.Dtos.TestAppointment
{
    public class TestAppointmentByTestTypeDto
    {
        public int TestAppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public decimal PaidFees { get; set; }
        public bool IsLocked { get; set; }
    }
}
