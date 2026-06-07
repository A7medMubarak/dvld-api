namespace DVLD.Contracts.Dtos.DetainedLicense
{
    public class DetainedLicenseDto
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

        public DetainedLicenseDto() { }

        public DetainedLicenseDto(int detainId, int licenseId, DateTime detainDate,
            decimal fineFees, int createdByUserId, bool isReleased,
            DateTime? releaseDate, int? releaseByUserId, int? releaseApplicationId)
        {
            DetainId = detainId;
            LicenseId = licenseId;
            DetainDate = detainDate;
            FineFees = fineFees;
            CreatedByUserId = createdByUserId;
            IsReleased = isReleased;
            ReleaseDate = releaseDate;
            ReleaseByUserId = releaseByUserId;
            ReleaseApplicationId = releaseApplicationId;
        }
    }
}
