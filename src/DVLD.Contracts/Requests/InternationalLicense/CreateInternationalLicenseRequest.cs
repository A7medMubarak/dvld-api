namespace DVLD.Contracts.Requests.InternationalLicense
{
    public class CreateInternationalLicenseRequest
    {
        public int IssuedUsingLocalLicenseId { get; set; }
        public int DriverId { get; set; }
    }
}
