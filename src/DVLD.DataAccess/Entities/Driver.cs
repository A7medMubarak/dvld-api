namespace DVLD.DataAccess.Entities
{
    public class Driver
    {
        public int DriverId { get; set; }
        public int PersonId { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }

        public Person Person { get; set; } = null!;
        public User CreatedByUser { get; set; } = null!;
        public ICollection<License> Licenses { get; set; } = new List<License>();


    }
}