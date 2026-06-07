using DVLD.Contracts.Common.Enums;

namespace DVLD.DataAccess.Entities
{
    public class Application
    {
        public int ApplicationId { get; set; }
        public int ApplicantPersonId { get; set; }
        public DateTime ApplicationDate { get; set; }
        public int ApplicationTypeId { get; set; }
        public enApplicationStatus ApplicationStatus { get; set; }
        public DateTime LastStatusDate { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserId { get; set; }

        // Navigation properties
        public Person ApplicantPerson { get; set; } = null!;
        public User CreatedByUser { get; set; } = null!;
        public ApplicationType ApplicationType { get; set; } = null!;
        public LocalDrivingLicenseApplication? LocalDrivingLicenseApplication { get; set; }
        public License? License { get; set; }
        public InternationalLicense? InternationalLicense { get; set; }
    }
}
