namespace DVLD.Contracts.Dtos.License
{
    public class DriverLicensesDto
    {
        public int LicenseId { get; set; }
        public int ApplicationId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }

        public DriverLicensesDto() { }

        public DriverLicensesDto(int licenseId, int applicationId, string className,
            DateTime issueDate, DateTime expirationDate, bool isActive)
        {
            LicenseId = licenseId;
            ApplicationId = applicationId;
            ClassName = className;
            IssueDate = issueDate;
            ExpirationDate = expirationDate;
            IsActive = isActive;
        }
    }
}
