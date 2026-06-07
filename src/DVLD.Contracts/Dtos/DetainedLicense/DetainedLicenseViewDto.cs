namespace DVLD.Contracts.Dtos.DetainedLicense
{
    public class DetainedLicenseViewDto
    {
        public int DetainId { get; set; }
        public int LicenseId { get; set; }
        public DateTime DetainDate { get; set; }
        public bool IsReleased { get; set; }
        public decimal FineFees { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? NationalNo { get; set; }
        public string? FullName { get; set; }
        public int? ReleaseApplicationId { get; set; }

        public DetainedLicenseViewDto() { }

        public DetainedLicenseViewDto(int detainId, int licenseId, DateTime detainDate,
            bool isReleased, decimal fineFees, DateTime? releaseDate,
            string? nationalNo, string? fullName, int? releaseApplicationId)
        {
            DetainId = detainId;
            LicenseId = licenseId;
            DetainDate = detainDate;
            IsReleased = isReleased;
            FineFees = fineFees;
            ReleaseDate = releaseDate;
            NationalNo = nationalNo;
            FullName = fullName;
            ReleaseApplicationId = releaseApplicationId;
        }
    }
}
