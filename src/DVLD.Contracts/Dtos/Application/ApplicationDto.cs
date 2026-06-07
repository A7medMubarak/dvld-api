namespace DVLD.Contracts.Dtos.Application
{
    public record ApplicationDto
    {
        public int ApplicationId { get; set; }
        public int ApplicantPersonId { get; set; }
        public DateTime ApplicationDate { get; set; }
        public int ApplicationTypeId { get; set; }
        public byte ApplicationStatus { get; set; }
        public DateTime LastStatusDate { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserId { get; set; }

        public ApplicationDto() { }

        public ApplicationDto(int applicationId, int applicantPersonId, DateTime applicationDate,
            int applicationTypeId, byte applicationStatus, DateTime lastStatusDate,
            decimal paidFees, int createdByUserId)
        {
            ApplicationId = applicationId;
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
