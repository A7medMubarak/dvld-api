namespace DVLD.Contracts.Requests.Application
{
    public class CreateApplicationRequest
    {
        public int ApplicantPersonId { get; set; }
        public int ApplicationTypeId { get; set; }
        public byte ApplicationStatus { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserId { get; set; }

        public CreateApplicationRequest() { }

        public CreateApplicationRequest(int applicantPersonId, int applicationTypeId,
            byte applicationStatus, decimal paidFees, int createdByUserId)
        {
            ApplicantPersonId = applicantPersonId;
            ApplicationTypeId = applicationTypeId;
            ApplicationStatus = applicationStatus;
            PaidFees = paidFees;
            CreatedByUserId = createdByUserId;
        }
    }
}
