namespace DVLD.DataAccess.Entities
{
    public class DetainedLicense
    {
        public int DetainId { get; set; }
        public int LicenseId { get; set; }
        public DateTime DetainDate { get; set; }
        public decimal FineFees { get; set; }
        public int CreatedByUserId { get; set; }
        public bool IsReleased { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? ReleaseByUserId { get; set; }
        public int? ReleaseApplicationId { get; set; }

        public License License { get; set; } = null!;
        public User CreatedByUser { get; set; } = null!;
        public User? ReleaseByUser { get; set; }
        public Application? ReleaseApplication { get; set; }
    }

   
}
