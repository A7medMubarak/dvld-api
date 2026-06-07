namespace DVLD.Contracts.Dtos.InternationalLicense
{
    public class InternationalLicenseDto
    {
        public int InternationalLicenseId { get; set; }
        public int ApplicationId { get; set; }
        public int DriverId { get; set; }
        public int IssuedUsingLocalLicenseId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }
        public int CreatedByUserId { get; set; }

        public InternationalLicenseDto() { }

        public InternationalLicenseDto(int internationalLicenseId, int applicationId, int driverId,
            int issuedUsingLocalLicenseId, DateTime issueDate, DateTime expirationDate,
            bool isActive, int createdByUserId)
        {
            InternationalLicenseId = internationalLicenseId;
            ApplicationId = applicationId;
            DriverId = driverId;
            IssuedUsingLocalLicenseId = issuedUsingLocalLicenseId;
            IssueDate = issueDate;
            ExpirationDate = expirationDate;
            IsActive = isActive;
            CreatedByUserId = createdByUserId;
        }
    }
}
