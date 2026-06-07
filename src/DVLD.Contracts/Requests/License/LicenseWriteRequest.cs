namespace DVLD.Contracts.Requests.License
{
    public abstract class LicenseWriteRequest
    {
        public int ApplicationId { get; set; }
        public int DriverId { get; set; }
        public int LicenseClassId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string? Notes { get; set; }
        public decimal PaidFees { get; set; }
        public bool IsActive { get; set; }
        public byte IssueReason { get; set; }
    }
}
