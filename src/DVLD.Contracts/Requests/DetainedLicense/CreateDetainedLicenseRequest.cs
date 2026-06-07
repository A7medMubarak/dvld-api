namespace DVLD.Contracts.Requests.DetainedLicense
{
    public class CreateDetainedLicenseRequest
    {
        public int LicenseId { get; set; }
        public decimal FineFees { get; set; }
    }
}
