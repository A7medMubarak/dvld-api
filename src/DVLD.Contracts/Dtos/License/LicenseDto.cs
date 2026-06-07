using DVLD.Contracts.Common.Enums;

namespace DVLD.Contracts.Dtos.License
{
    public class LicenseDto
    {
        public int LicenseId { get; set; }
        public int ApplicationId { get; set; }
        public int DriverId { get; set; }
        public int LicenseClassId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string? Notes { get; set; }
        public decimal PaidFees { get; set; }
        public bool IsActive { get; set; }
        public enIssueReason IssueReason { get; set; }
        public int CreatedByUserId { get; set; }

        public LicenseDto() { }

        public LicenseDto(int licenseId, int applicationId, int driverId, int licenseClassId,
            DateTime issueDate, DateTime expirationDate, string? notes, decimal paidFees,
            bool isActive, enIssueReason issueReason, int createdByUserId)
        {
            LicenseId = licenseId;
            ApplicationId = applicationId;
            DriverId = driverId;
            LicenseClassId = licenseClassId;
            IssueDate = issueDate;
            ExpirationDate = expirationDate;
            Notes = notes;
            PaidFees = paidFees;
            IsActive = isActive;
            IssueReason = issueReason;
            CreatedByUserId = createdByUserId;
        }
    }
}
