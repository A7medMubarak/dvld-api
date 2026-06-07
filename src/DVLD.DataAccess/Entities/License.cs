using DVLD.Contracts.Common.Enums;

namespace DVLD.DataAccess.Entities
{
    public class License
    {
        public int LicenseId { get; set; }
        public int LicenseClassId { get; set; }
        public int ApplicationId { get; set; }
        public int DriverId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public enIssueReason IssueReason { get; set; }
        public string? Notes { get; set; }
        public decimal PaidFees { get; set; }
        public bool IsActive { get; set; }
        public int CreatedByUserId { get; set; }

        public User CreatedByUser { get; set; } = null!;
        public Application Application { get; set; } = null!;
        public LicenseClass LicenseClass { get; set; }  = null!;
        public Driver Driver { get; set; } = null!;
        public ICollection<DetainedLicense> DetainedLicenses { get; set; } = new List<DetainedLicense>();

    }
}
