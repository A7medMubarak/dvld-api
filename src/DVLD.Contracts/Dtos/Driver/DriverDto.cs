namespace DVLD.Contracts.Dtos.Driver
{
    public class DriverDto
    {
        public int DriverId { get; set; }
        public int PersonId { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }

        public DriverDto() { }

        public DriverDto(int driverId, int personId, int createdByUserId, DateTime createdDate)
        {
            DriverId = driverId;
            PersonId = personId;
            CreatedByUserId = createdByUserId;
            CreatedDate = createdDate;
        }
    }
}
