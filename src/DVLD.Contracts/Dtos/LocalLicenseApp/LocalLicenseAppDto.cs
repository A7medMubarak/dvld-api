namespace DVLD.Contracts.Dtos.LocalLicenseApp
{
    public class LocalLicenseAppDto
    {
        public int LocalDrivingLicenseApplicationId { get; set; }
        public int ApplicationId { get; set; }
        public int LicenseClassId { get; set; }
        public int ApplicantPersonId { get; set; }
        public DateTime ApplicationDate { get; set; }
        public int ApplicationTypeId { get; set; }
        public byte ApplicationStatus { get; set; }
        public DateTime LastStatusDate { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserId { get; set; }

        public LocalLicenseAppDto() { }

        public LocalLicenseAppDto(int localDrivingLicenseApplicationId, int applicationId, int licenseClassId,
            int applicantPersonId, DateTime applicationDate, int applicationTypeId,
            byte applicationStatus, DateTime lastStatusDate, decimal paidFees, int createdByUserId)
        {
            LocalDrivingLicenseApplicationId = localDrivingLicenseApplicationId;
            ApplicationId = applicationId;
            LicenseClassId = licenseClassId;
            ApplicantPersonId = applicantPersonId;
            ApplicationDate = applicationDate;
            ApplicationTypeId = applicationTypeId;
            ApplicationStatus = applicationStatus;
            LastStatusDate = lastStatusDate;
            PaidFees = paidFees;
            CreatedByUserId = createdByUserId;
        }
    }
}
