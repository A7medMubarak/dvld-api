namespace DVLD.Contracts.Dtos.LocalLicenseApp
{
    public class LocalLicenseViewDto
    {
        public int LocalDrivingLicenseApplicationId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string NationalNo { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime ApplicationDate { get; set; }
        public int PassedTestCount { get; set; }
        public string Status { get; set; } = string.Empty;

        public LocalLicenseViewDto() { }

        public LocalLicenseViewDto(int localDrivingLicenseApplicationId, string className,
            string nationalNo, string fullName, DateTime applicationDate,
            int passedTestCount, string status)
        {
            LocalDrivingLicenseApplicationId = localDrivingLicenseApplicationId;
            ClassName = className;
            NationalNo = nationalNo;
            FullName = fullName;
            ApplicationDate = applicationDate;
            PassedTestCount = passedTestCount;
            Status = status;
        }
    }
}
