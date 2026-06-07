using DVLD.Contracts.Dtos.Person;

namespace DVLD.Contracts.Dtos.Driver
{
    public class DriverDetailedDto
    {
        public int DriverId { get; set; }
        public int PersonId { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public PersonDto? Person { get; set; }
    }
}
