namespace DVLD.Contracts.Requests.Application
{
    public class UpdateApplicationRequest
    {
        public int ApplicantPersonId { get; set; }
        public DateTime ApplicationDate { get; set; }
        public int ApplicationTypeId { get; set; }
        public byte ApplicationStatus { get; set; }
        public DateTime LastStatusDate { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserId { get; set; }
    }
}
